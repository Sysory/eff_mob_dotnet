# from random import randint as rd
from random import random
"""Файл для генерации входных данных
(лог, содержащий ip адреса и время, сортировка по времени)"""

"""ТЗ: входные данные: На каждой строке записан адрес и время, в которое с него пришёл запрос.
Адрес и время доступа разделено двоеточием. 
Дата в журнале доступа записана в формате: yyyy-MM-dd HH:mm:ss
xxx.xxx.xxx.xxx:yyyy-MM-dd HH:mm:ss
"""

def fastRd(a: int, b: int) -> int:
  return round(random() * (b - a) + a)

class Entry:
  def __init__(self):
    daysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
    date = [fastRd(2022, 2024), fastRd(1, 12), fastRd(0,31)]
    date[2] = min(date[2], daysInMonth[date[1] - 1])

    self.entry = {
      "ip": [fastRd(0, 255) for _ in range(4)],
      "date": date, # yyyy, MM, dd
      "time": [fastRd(0, 23), fastRd(0, 59), fastRd(0,59)] # HH, mm, ss
    }
  
  def formatted(self) -> str:
    ip = self.entry["ip"]
    date = self.entry["date"]
    time = self.entry["time"]
    f = f"{ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}:%4d-%02d-%02d %02d:%02d:%02d\n" % (*date, *time)
    return f

def timeDateSortKey(en: Entry) -> int:
  time = en.entry["time"]
  date = en.entry["date"]

  sec = time[2]
  minute = time[1] * 1e2
  hour = time[0] * 1e4
  day = date[2] * 1e6
  month = date[1] * 1e8
  year = date[0] * 1e12

  key = sec + minute + hour + day + month + year
  return key

def main():
  entries: list[Entry] = [Entry() for _ in range(100)] #1e5
  entries.sort(key=timeDateSortKey)

  entries: list[str] = [x.formatted() for x in entries]
  with (open("addresses.txt", "w")) as file:
    file.writelines(entries)

if __name__ == "__main__":
  main()