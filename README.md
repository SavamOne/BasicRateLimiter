## Домашнее задание по курсу Route256

Реализован RateLimiter, использующий алгоритм Fixed window
- Ограничение по конкретному пути, ограничение по IP и/или общее ограничение
- Хранение настроек в БД или в appsettings.json
- Конфигурирование выполняется в одном extension-методе для ConfigureServices и добавлением Middleware
