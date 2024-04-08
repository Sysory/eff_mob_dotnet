# Сводка

## Про общее выполнение

Во-первых, что хочу заметить. В ТЗ есть лично для меня некоторые непонятки, такие как формат передаваемых даты и времени в аргументе командной строки (я использовал разделитель T (датаTвремя)), формулировка "Адрес и время доступа разделено двоеточием" указана после уточнений формата вывода (не понятно, это относится к выводу или к фходному лог файлу (скорее последнее, но не уверен до конца)).

В конце я начал торопиться, т.к. мне не хватило времени, чтобы реализовать всё так, как я хотел, поэтому архитектура и логика много где страдают, но для меня отрефакторить и поправить это не проблема, было бы еще время.

## Моменты

Я успел только сделать минимально рабочую программу. Есть реализация получения флагов через env переменные, но проверить её работоспособность я не успел, как и не успел прописать изначально задуманные мной тесты. Не известно, как поведёт себя код, если скормить ему не валидный лог (по реализации предполагаю, что просто выдаст пустой файл на выходе, если в не валидном логе не будет ни одной валидной строки, соответствующей шаблону)

### Переменные среды

Переменные среды для передачи аргументов (справа env var, слева устанавливаемое значение):

    { "file-log", "EM_FILE_LOG" },
    { "file-output", "EM_FILE_OUTPUT" },
    { "address-start", "EM_ADDRESS_START" },
    { "address-mask", "EM_ADDRESS_MASK" },
    { "time-start", "EM_TIME_START" },
    { "time-end", "EM_TIME_END" }

По умолчанию, консольные аргументы в приоритете над переменнами среды.
