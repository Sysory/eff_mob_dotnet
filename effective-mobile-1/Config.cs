using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace effectivemobile {
    class Config {
        // TODO свойства стоит хранить как класс Props с именем, значением, дефолт значением для каждого свойства
        public Dictionary<String, String> props;
        private readonly List<IBaseConfig> confParsers;
        // TODO небыло смысла делить флаги. нужно добавить - и -- к именам флагов и оставить 1 список
        protected String[] longFlags = ["file-log", "file-output", "address-start", "address-mask", "time-start", "time-end"];
        protected String[] shortFlags = [];
        protected String[] noValueFlags = [];

        public Config() {
            confParsers = new List<IBaseConfig>();
            props = new Dictionary<string, string>();

            foreach (var flag in longFlags.Concat(shortFlags)) {
                props.Add(flag, "");
            }
        }

        public virtual Dictionary<String, String> ParseProperties() {
            foreach (var name in longFlags) {
                String value = "";
                foreach (var parser in confParsers) {
                    value = parser.ParseArg(name);
                    if (value != "") break;
                }
                props[name] = value;
            }
            // defaul values
            if (props["address-start"] == "") props["address-start"] = "0.0.0.0";
            if (props["address-mask"] == "") props["address-mask"] = "0";
            return props;
        }

        public void AddConfigParser(IBaseConfig conf) {
            confParsers.Add(conf);
        }
    }
    interface IBaseConfig {
        public abstract String ParseArg(String name);
    }

    class SystemConfig : Config, IBaseConfig {
        readonly String[] args;
        public SystemConfig(String[] args) {
            this.args = args;
            try {
                CheckUndefinedFlags();
                CheckArgsOrder();
            } catch (ArgumentException e) {
                Console.WriteLine($"Ошибка передачи аргументов: {e.Message}");
                Environment.Exit(1);
            } catch (Exception e) {
                Console.WriteLine($"Неизвестная ошибка: {e.Message}\n{e.Data}");
                Environment.Exit(1);
            }
        }

        protected virtual void CheckUndefinedFlags() {
            foreach (var arg in args) {
                // if (IsFlag(arg) && !(shortFlags.Concat(longFlags).Concat(noValueFlags).Contains(arg)) ) {
                if (IsFlag(arg) && !longFlags.Contains(arg[2..]) ) {
                    throw new ArgumentException($"Неизвестный флаг: {arg}");
                }
            }
        }

        protected virtual void CheckArgsOrder() {
            if (!args[0].StartsWith("-")) {
                throw new ArgumentException($"Ожидался флаг, не значение");
            }
            for (int i = 0; i < args.Length - 1; i++) {
                var arg = args[i];
                var nextArg = args[i+1];
                if (noValueFlags.Contains(arg[2..]))
                    continue;
                if (IsFlag(arg) && IsFlag(nextArg)) {
                    throw new ArgumentException($"Ожидалось значение, не флаг '{nextArg}'");
                }

                if (!IsFlag(arg) && !IsFlag(nextArg)) {
                    throw new ArgumentException($"Ожидался флаг, не значение '{nextArg}'");
                } 
            }
        }

        public String ParseArg(String name) {
            String value = "";
            for (int i = 0; i < args.Length; i++) {
                if (args[i].StartsWith("--")) {
                    String arg = args[i][2..];
                    if (arg.Equals(name)) {
                        value = args[i+1];
                        break;
                    }
                }
            }
            return value;
        }

        private static bool IsFlag(String arg) {
            return IsShortFlag(arg) || IsLongFlag(arg);
        }

        private static bool IsLongFlag(String arg) {
            return arg.StartsWith("--") && !arg.StartsWith("---");
        }

        private static bool IsShortFlag(String arg) {
            return arg.StartsWith("-") && !arg.StartsWith("--");
        }
    }
    
    class EnvirConfig : IBaseConfig {
        protected Dictionary<String, String> envKeys;
        
        public EnvirConfig() {
            envKeys = new Dictionary<string, string>
            {
                { "file-log", "EM_FILE_LOG" },
                { "file-output", "EM_FILE_OUTPUT" },
                { "address-start", "EM_ADDRESS_START" },
                { "address-mask", "EM_ADDRESS_MASK" },
                { "time-start", "EM_TIME_START" },
                { "time-end", "EM_TIME_END" }
            };
        }
        
        public String ParseArg(String name) {
            String? value = Environment.GetEnvironmentVariable(envKeys[name]);
            if (value == null) return "";
            else return value;
        }
    }
}
