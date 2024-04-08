using System.Collections;
using System.Net;

namespace effectivemobile {
    class MainProgramm {
        static int Main(String[] args) {
            Config conf = new Config();
            conf.AddConfigParser(new SystemConfig(args));
            conf.AddConfigParser(new EnvirConfig());
            conf.ParseProperties();
            // TODO можно описать интерфейс для Config и передавать его в ConfigValidator
            // т.к. сейчас ConfigValidator сильно привязан к Config
            ConfigValidator cv = new ConfigValidator(conf.props);
            cv.Validate();

            IFilter filter = new LogFilter(conf);

            // FileStream log = File.OpenRead(conf.props["file-log"]);
            
            Hashtable ht = new Hashtable();
                foreach(var line in File.ReadLines(conf.props["file-log"])) {
                    if (filter.Validate(line)) {
                        String ip = line.Split(':')[0];
                        if (!ht.Contains(ip)) {
                            ht.Add(ip, 1);
                        } else {
                            ht[ip] = (int?)ht[ip] + 1;
                        }
                    }
                }

            using (StreamWriter output = new StreamWriter(conf.props["file-output"])) {
                foreach (var key in ht.Keys) {
                    output.Write($"{key} {ht[key]}\n");
                }
                output.Close();
            }
            Console.WriteLine("Done!");
            return 0;
        }
    }
}