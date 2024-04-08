using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace effectivemobile {
    interface IFilter {
        public bool Validate(String? line);
    }

    class LogFilter : IFilter{
        private readonly Config conf;
        private DateTime minDateTime;
        private DateTime maxDateTime;
        private IPAddress? minIP;
        private IPAddress? maxIP;

        public LogFilter(Config config) {
            conf = config;
            Init();
        }

        private void Init() {
            minDateTime = DateTime.ParseExact(
                conf.props["time-start"],
                "dd.MM.yyyyTHH:mm:ss",
                null
            );
            maxDateTime = DateTime.ParseExact(
                conf.props["time-end"],
                "dd.MM.yyyyTHH:mm:ss",
                null
            );

            String minIPstr = conf.props["address-start"];
            IPAddress.TryParse(minIPstr, out minIP);

            int mask = int.Parse(conf.props["address-mask"]);
            maxIP = MaxIP(minIP, mask);
        }
        // проверить одну запись лога, входит ли она в диапазон времени
        public bool Validate(String? logLine) {
            try {
                if (logLine == null) return false;
                String logIPstr = logLine.Split(':')[0];
                if (!IPAddress.TryParse(logIPstr, out var logIP)) return false;

                int cmpToMin = CompareIP(minIP, logIP);
                int cmpToMax = CompareIP(maxIP, logIP);

                if (cmpToMin == 1 || cmpToMin == -1) return false;
                if (cmpToMax == 2 || cmpToMax == -1) return false;
                
                // 94.152.166.33:2022-01-00 06:31:52
                // yyyy-MM-dd HH:mm:ss
                String logDateTimestr = logLine.Substring(logLine.IndexOf(':') + 1);
                DateTime logDateTime = DateTime.ParseExact(
                    logDateTimestr,
                    "yyyy-MM-dd HH:mm:ss",
                    null
                );

                if (logDateTime < minDateTime || logDateTime > maxDateTime) return false;
            } catch (Exception) {
                return false;
            }
            return true;
        }

        // TODO метод нарушает принцип единой ответственности, стоит вынести в отдельный класс операций с ip
        //  (или можно расширить класс IPAddress и добавить методы)
        // максимальный ip по маске, включая broadcast
        public static IPAddress MaxIP(IPAddress ip, int mask) {
            Byte[] ipb = ip.GetAddressBytes();
            Byte[] maxIPb = new Byte[4];

            int fullBytes = mask / 8;
            for (int i = 0; i < fullBytes; i++) {
                maxIPb[i] = ipb[i];
            }
            
            var Zeroes = delegate(int num) {
                return (1 << num) - 1;
            };

            int restByte = mask % 8;
            if (fullBytes < 4) {
                int tmp = (Zeroes(restByte)) << (8 - restByte);  // net byte mask
                tmp = ipb[fullBytes] & tmp;                     // net byte
                tmp |= Zeroes(8 - restByte);                   // установил биты младше net byte в 1
                maxIPb[fullBytes] = (Byte) tmp;

                for (int i = fullBytes + 1; i < 4; i++) {
                    maxIPb[i] = (Byte) 255;
                }
            }

            return new IPAddress(maxIPb);
        }

        // TODO метод нарушает принцип единой ответственности, стоит вынести в отдельный класс операций с ip
        //  (или можно расширить класс IPAddress и добавить методы)
        protected virtual int CompareIP(IPAddress? ip1, IPAddress? ip2) {
            try {
                Byte[] ip1b = ip1.GetAddressBytes();
                Byte[] ip2b = ip2.GetAddressBytes();

                bool eq = true;
                for (int i = 0; i < 4; i++) {
                    if (ip1b[i] != ip2b[i]) {
                        eq = false;
                        break;
                    }
                }
                if (eq) return 0;


                if (ip1b[0] > ip2b[0]) return 1;
                if (ip1b[0] < ip2b[0]) return 2;

                if (ip1b[1] > ip2b[1]) return 1;
                if (ip1b[1] < ip2b[1]) return 2;

                if (ip1b[2] > ip2b[2]) return 1;
                if (ip1b[2] < ip2b[2]) return 2;

                if (ip1b[3] > ip2b[3]) return 1;
                if (ip1b[3] < ip2b[3]) return 2;

                return 0;
            } catch (Exception) {
                return -1;
            }
        }
    }
}
