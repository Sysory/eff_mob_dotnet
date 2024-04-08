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
            
            int c = 0;
            using (StreamWriter output = new StreamWriter(conf.props["file-output"])) {
                foreach(var line in File.ReadLines(conf.props["file-log"])) {
                    if (filter.Validate(line)) {
                        c++;
                        output.WriteLine(line);
                    }
                }
                output.Close();
            }
            Console.WriteLine($"Done! {c}");
            return 0;
        }
    }
}