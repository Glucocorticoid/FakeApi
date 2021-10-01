﻿# Тестовый ASP.NET MVC WebApi проект

Это репорзиторий тестового проекта по реализации WebApi сервиса на основе реального тестового задания.
В связи с недостаточной детализацией тестового задания и специфическими требованиями к 
реализации отдельных механизмов работы приложения, код проекта содержит "странные" технические решения и допущения,
которые в действующих проектах должны быть реализованы иным (более практичным) образом.

# Функциональное тестирование проекта

Приложение доступно для тестирования локально по адресу http://localhost:5000/

Для тестирования POST-запросов по адресу http://localhost:5000/report/user_statistics можно использовать, например,
консоль Package Manager из Visual Studio, путем исполнения следующей команды:
	PM> Invoke-RestMethod http://localhost:5000/report/user_statistics -Method POST -Body (@{user_id = "TestUserID"; timeFrom = "2021-07-26T00:00:00"; timeTo = "2021-07-29T00:00:00"  } | ConvertTo-Json) -ContentType "application/json; charset=utf-8"

В качестве ответа на корректные исходные данные в консоль должен поступить ответ в виде GUID идентификатора:
	56029acf-663c-4bca-b63d-9a157c73c100

Для тестирования GET-запросов по адресу можно использовать например, интенет-браузер с указанием в адресной строке браузера адреса для запроса:
	http://localhost:5000/report/info?query=56029acf-663c-4bca-b63d-9a157c73c100
В качестве ответа на исходные данные в окне браузера должен отобразиться JSON-объект, например:
	{"query":"56029acf-663c-4bca-b63d-9a157c73c100","percent":100,"result":{"user_id":"TestUserID","count_sign_in":"12"}}

# Требования и допущения

Согласно требований к реализации проекта, приложение "должно реализовывать асинхронную обработку запросов на .net core, тип приложения WebApi".

Сервис получает POST запросы по адресу /report/user_statistics. 
Поскольку формат тела запроса не определен, примем следующие допущения:
- данные передаются в JSON формате
- "идентификатор пользователя" не ограничен форматом и представляет собой произвольную строку
- "период с и по" представляет собой две метки времени в формате DateTime
- наименование полей JSON-объекта примем как user_id, timeFrom, timeTo
- возвращаемые данные в виде GUID запроса представляю JSON-объект вида { "result" : "guid" }

Сервис возвращает информацию о состоянии запросов в результате GET запроса по адресу /report/info.
Входной параметр - GUID идентификатор, полученные в ответ на POST запрос по адресу /report/user_statistics.
Исходя из примеров возвращаемых данных примем следующие допущения:
- метод получает информацию из заголовка запроса в формате /report/info?query=guid
- сущность поля count_sign_in не определена и возвращается строковое значение "12" в любом случае

Поскольку формат ответа WebApi сервиса на некорректные входные данные тестовым заданием не определен,
в качестве ответа на такие запросы в обоих случаях возвращается пустой JSON объект {}.

# Состав проекта

Тестовое приложение размещено в проекте FakeTestApp. 
Логика обработки запросов реализована в MVC-контроллере UserStatisticsController, размещенном в папке Controllers. 
Размещенные в папке Models модели представляют объекты пользовательских запросов и ответов, а также объект записей из базы данных.
Размещенные в папке Services сервисы реализуют механизмы доступа к базе данных и настройкам приложения.
В проекте FakeTestApp.Tests размещены unit-тесты для методов класса UserStatisticsController.

# Реализация особых требований

## Время обработки запроса
Поступающие POST запросы по адресу /report/user_statistics проверяются на корректность данных, 
получают метку времени поступления запроса и помещаются в базу данных.

При поступлении GET запроса по адресу /report/info информация о запросе извлекается из базы данных,
выполняется анализ времени между GET и POST запросами, на основе анализа возвращается процент обработки запроса.

Длительнсть обработки запроса задается в конфигурации приложения appsettings.json (MaxRequestExcecutionTime) в миллисекундах (значение по умолчанию - 60000 мс).
Таким образом реализуется требование о расчёте "процента обработки в зависимости от пройденного времени с момента создания запроса", 
а также требование "обрабатывать запрос не быстрее чем за Х миллисекунд (вынести в конфигурационный файл, по умолчанию установить 60 секунд)".
Кроме того, поскольку информация о времени поступления POST запроса хранится независимо в базе данных, реализуется требование 
"если приложение перезагрузить, информацию о запросе не должна быть потеряна".

## Использование ORM

В проекте применена ORM-библиотека Entity Framework Core, поддерживающая миграции базы данных.
Миграции, инициализирующие базу данных, размещены в папке Migrations проекта FakeTestApp.
В качестве базы данных используется MSSQL/LocalDB.

## Асинхронность запросов

Для обеспечения асинхронности обработки запросов применяется асинхронная модель на основе задач (TAP).
Методы обработки запросов контроллера UserStatisticsController возвращают объекты типа Task,
используются встроенные механизмы языка C# (async/await) для асинхронной работы методов контроллера.
Кроме того, при работе с Entity Framework вместо встроенных асинхронных методов чтения/записи данных,
в классе RequestContext реализованы асинхронные обёртки (Task.Run) над синхронными методами доступа к данным.
Таким образом реализуется требование "готовые процессоры асинхронных запросов не использовать".

## Покрытие тестами

Тестирование приложения реализовано в виде отдельного проекта FakeTestApp.Tests.
Для тестирования функционала приложения использованиа библиотека unit-тестирования NUnit.
Изоляция тестируемого кода от сторонних зависимостей реализована при помощи mock-библиотеки Moq.
Unit-тесты покрывают код методов Get и Post контроллера UserStatisticsController, обрабатывающего Api-запросы.
Тестирование подразумевает проверку корректности работы тестируемых методов в зависимости от корректности входных данных.