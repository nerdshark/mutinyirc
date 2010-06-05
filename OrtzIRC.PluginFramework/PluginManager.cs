﻿namespace OrtzIRC.PluginFramework
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using OrtzIRC.Common;
    using System.Linq;

    /// <summary>
    /// Manages plugins and commands.
    /// </summary>
    public sealed class PluginManager
    {
        private static Dictionary<string, CommandInfo> commands;
        private static List<PluginInfo> plugins;

        private static string userPluginPath;

        private PluginManager()
        {
            plugins = new List<PluginInfo>();
            commands = new Dictionary<string, CommandInfo>();
        }

        public static PluginManager Instance { get; private set; }

        /// <summary>
        /// Instantiates the PluginManager and loads any plugins found.
        /// </summary>
        /// <remarks>Must be called first</remarks>
        public static void LoadPlugins(string pluginPath)
        {
            if (Instance == null)
                Instance = new PluginManager();

            userPluginPath = pluginPath;

            FindPlugins();
        }

        /// <summary>
        /// Searches the plugins directory for assemblies, examines them for plugins and populates
        /// </summary>
        private static void FindPlugins()
        {
            Trace.WriteLine("Loading Plug-ins", TraceCategories.PluginSystem);

            string[] files = System.IO.Directory.GetFileSystemEntries(userPluginPath, "*.dll");

            foreach (string file in files)
            {
                try
                {
                    foreach (PluginInfo info in AssemblyExaminer.ExamineAssembly(Assembly.LoadFrom(file)))
                    {
                        if (info is CommandInfo)
                        {
                            if (!commands.ContainsKey(info.FullName))
                            {
                                commands.Add(info.FullName, info as CommandInfo);
                                Trace.WriteLine(string.Format("Added command plugin {0} at {1}", info.FullName, info.AssemblyPath), TraceCategories.PluginSystem);
                            }
                            else
                            {
                                Trace.WriteLine(string.Format("Could not load command {0}. A command by that name already exists.", info.FullName),
                                    TraceCategories.PluginSystem);
                                //TODO: Should let user know about this.
                            }
                        }
                        else
                        {
                            plugins.Add(info);
                            Trace.WriteLine("Added plugin " + info.FullName + " at " + info.AssemblyPath, TraceCategories.PluginSystem);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("Could not load {0}{1}{2}", file, Environment.NewLine, ex), TraceCategories.PluginSystem);
                }
            }
            Trace.WriteLine("Finished loading Plug-ins", TraceCategories.PluginSystem);
        }

        private static ICommand GetCommandInstance(string name)
        {
            foreach (KeyValuePair<string, CommandInfo> item in commands)
            {
                if (item.Value.CommandName.ToUpper() == name.ToUpper())
                    return (ICommand)CreateInstance(item.Value);
            }
            Trace.WriteLine(String.Format("No command called {0} found", name.ToUpper()), TraceCategories.PluginSystem);
            return null;
        }

        private static IPlugin CreateInstance(PluginInfo pluginInfo)
        {
            Assembly asm = Assembly.LoadFile(pluginInfo.AssemblyPath);

            return (IPlugin)asm.CreateInstance(pluginInfo.FullName);
        }

        public static CommandResultInfo ExecuteCommand(CommandExecutionInfo info)
        {
            //TODO: This should handle errors
            //TODO: Pretty complex, maybe could use some commenting

            ICommand commandInstance = GetCommandInstance(info.Name);
            if (commandInstance == null)
                return new CommandResultInfo
                           {
                               Message = String.Format("{0} is an invalid command", info.Name.ToUpper()),
                               Result = CommandResult.Fail
                           };

            var methods = commandInstance.GetType().GetMethods()
            .Where(o => o.Name == "Execute")
            .Where(o => o.GetParameters()[0].ParameterType.BaseType == typeof(MessageContext));

            MethodInfo[] methodInfos = methods.ToArray();

            for (int i = 0; i < methodInfos.Length; i++) //Loop through the methods
            {
                ParameterInfo[] methodParameters = methodInfos[i].GetParameters();

                for (int j = 0; j < methodParameters.Length; j++) //Loop throught the method's parameters
                {
                    ParameterInfo methodParameter = methodParameters[j];

                    if (info.ParameterList.Count < methodParameters.Length - 1)
                        break;

                    if (j == 0)
                    {
                        if (methodParameter.ParameterType != info.Context.GetType()) //First parameter must be a context
                            break;

                        if (info.ParameterList.Count == 0) //Handle parameterless command
                        {
                            if (methodParameters.Length - 1 != 0)
                                break;

                            info.ParameterList.Insert(0, info.Context);
                            return (CommandResultInfo)methodInfos[i].Invoke(commandInstance, info.ParameterList.ToArray());
                        }
                        continue;
                    }

                    if (FlamingIRC.Rfc2812Util.IsValidChannelName(info.ParameterList[j - 1] as string))
                        info.ParameterList[j - 1] = new ChannelInfo(info.ParameterList[j - 1] as string);

                    var sp = (info.ParameterList[j - 1] as string);
                    if (sp != null && sp.StartsWith("-")) // Check for switches
                    {
                        sp = sp.Remove(0, 1);
                        info.ParameterList[j - 1] = sp.ToCharArray();
                    }

                    if (methodParameter.ParameterType != info.ParameterList[j - 1].GetType())
                        break; //Parameter mismatch. Break parameter loop and go the the next method

                    if (j != methodParameters.Length - 1) continue; //If this isn't the last parameter then keep looping

                    //Checks for an "open-ended" string parameter.
                    if (methodParameter.ParameterType == typeof(string))
                    {
                        bool allStrings = true;
                        System.Text.StringBuilder openString = new System.Text.StringBuilder();

                        int numberOpenEnded = 0;
                        for (int k = j - 1; k < info.ParameterList.Count; k++)
                        {
                            if (info.ParameterList[k].GetType() != typeof(string))
                            {
                                allStrings = false;
                                break;
                            }

                            openString.Append(info.ParameterList[k] + " ");
                            numberOpenEnded++;
                        }

                        if (allStrings && j != info.ParameterList.Count)
                        {
                            openString.Remove(openString.Length - 1, 1);
                            info.ParameterList.RemoveRange(j - 1, numberOpenEnded);
                            info.ParameterList.Add(openString.ToString());
                        }
                    }
                    info.ParameterList.Insert(0, info.Context);
                    return (CommandResultInfo)methodInfos[i].Invoke(commandInstance, info.ParameterList.ToArray());
                    //TODO: Should maybe log or something before returning
                }
            }
            return null;
        }

        public static CommandExecutionInfo ParseCommand(MessageContext context, string line)
        {
            if (line.StartsWith("/"))
            {
                string[] exploded = line.Split(new Char[] { ' ' });
                string name = exploded[0].TrimStart('/');
                string[] parameters = new string[exploded.Length - 1];
                Array.Copy(exploded, 1, parameters, 0, exploded.Length - 1); //Removing the first element
                return new CommandExecutionInfo
                {
                    Context = context,
                    Name = name,
                    ParameterList = new List<object>(parameters)
                };
            }
            else
            {
                string[] parameters = line.Split(new Char[] { ' ' });
                return new CommandExecutionInfo
                {
                    Context = context,
                    Name = "say",
                    ParameterList = new List<object>(parameters)
                };
            }
        }
    }
}
