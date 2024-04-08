using System.Globalization;
using System.IO;
using System.Net;
using System.Numerics;

namespace effectivemobile {
    class ConfigValidator {
        readonly Dictionary<String, String> props;
        public ConfigValidator(Dictionary<String, String> properties) {
            props = properties;
        }

/*
--file-log - путь к файлу лога (должен существовать файл)
--file-output - путь к файлу вывода (создаётся)
--address-start необязательный параметр - (ip адресс xxx.xxx.xxx.xxx, дефолт 0.0.0.0)
--address-mask Необязательный параметр - (целое число маска, дефолт 0)
--time-start - дата и время (dd.MM.yyyyTHH:mm:ss) (не описано, в каком виде передать в параметрах время)
--time-end - дата и время (dd.MM.yyyyTHH:mm:ss, больше чем time-start)
*/

/*
TODO Метод можно переименовать в ValidateAll, который бы вызывал метод Validate передаваемых ему
объектов класса отдельных проверок флагов, наследуемых от интерфейса базовой проверки флага с 1 методом, 
чтобы можно было переиспользовать методы проверок отдельных флагов.
*/
        public virtual void Validate() {
            try {
                bool res = false;
                res = CheckFileLog();
                if (!res) {
                    Console.WriteLine("Аргумент --file-log передан неверно");
                }
                res = CheckFileOutput();
                if (!res) {
                    Console.WriteLine("Аргумент --file-output передан неверно");
                    Environment.Exit(1);
                }
                res = CheckAddressStart();
                if (!res) {
                    Console.WriteLine("Аргумент --address-start передан неверно");
                    Environment.Exit(1);
                }
                res = CheckAddressMask();
                if (!res) {
                    Console.WriteLine("Аргумент --address-mask передан неверно");
                    Environment.Exit(1);
                }
                res = CheckTimeStartAndEnd();
                if (!res) {
                    Console.WriteLine("Аргумент --time-start или --time-end переданы неверно (dd.MM.yyyyTHH:mm:ss)");
                    Environment.Exit(1);
                }
                // if (!res) {
                //     // Environment.Exit(1);
                // }
            } catch (Exception e) {
                Console.WriteLine($"Неизвестная ошибка: {e.Message}");
                Environment.Exit(1);
            }
        }

        private bool CheckFileLog() {
            return File.Exists(props["file-log"]);
        }
        private bool CheckFileOutput() {
            return props["file-output"] != "";
        }
        private bool CheckAddressStart() {
            if (props["address-start"] == "0.0.0.0") return true; 
            return IPAddress.TryParse(props["address-start"], out IPAddress? tmp);
        }
        private bool CheckAddressMask() {
            bool res;
            // Console.WriteLine($"DEBUG {props["address-mask"]}");
            if (props["address-mask"] == "0") res = true;
            else {
                try {
                    res = int.TryParse(props["address-mask"], out int mask);
                    if (mask < 0 || mask > 32) res = false;
                } catch {
                    res = false;
                }
            }
            return res;
        }
        private bool CheckTimeStartAndEnd() {
            String dtStart = props["time-start"];
            String dtEnd = props["time-end"];
            String pattern = "dd.MM.yyyyTHH:mm:ss";

            bool resStart = DateTime.TryParseExact(dtStart, pattern, null, DateTimeStyles.None, out DateTime dateStart);
            if (!resStart) return false;
            bool resEnd = DateTime.TryParseExact(dtStart, pattern, null, DateTimeStyles.None, out DateTime dateEnd);
            if (!resEnd) return false;

            if (dateEnd < dateStart) return false;
            return true;
        }
    }
}
