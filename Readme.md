﻿# Описание библиотеки классов (коннектора) для получения кадровых данных из удаленного источника

## Используемые модули
Главный модуль: 
- Connector
Реализует интерфейс IConnector(3 эндпоинта). Служит для получения пользователем данных из удаленного источника.

Вспомогательные модули:
 - APISourceData
Реализует интерфейс ISourceData. Служит для получения данных из удаленного источника и преобразования их в список объектов.

- Validator
Реализует интерфейс IValidator. Служит для валидации полученных данных и сохранения их в память в случае успеха

- ArchiverData
Реализует интерфейс IArchiver. Служит для добавления в архив полученных валидных данных в виде 3 файлов формата *.json (employees.json, units.json, positions.json)

To Do: 
- Возможно стоит разбить главный метод в модуле Validator на более мелкие, но это вроде усложнит читабельность кода и не даст особого эффекта
