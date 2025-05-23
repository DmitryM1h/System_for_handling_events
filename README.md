Описание проекта:

Данный проект представляет собой систему для генерации и обработки событий, с последующей генерацией и хранением инцидентов на их основе. Система предназначена для мониторинга и анализа событий, таких как данные от камер, датчиков и других источников.

Цели
Разработать генератор событий, который будет постоянно генерировать события в случайные моменты времени.
Создать процессор событий, который будет обрабатывать сгенерированные события и создавать на их основе инциденты.
Реализовать API для взаимодействия с обоими сервисами.
Архитектура
Проект состоит из двух основных сервисов:

Генератор событий

Постоянно генерирует события в случайные моменты времени в 2-секундном интервале.
Имеет REST API (Swagger), позволяющий вручную генерировать события.
При генерации события отправляет его через HTTP-запрос в Процессор событий.

Процессор событий

Имеет REST API (Swagger) для получения событий и создания инцидентов на их основе.
Сохраняет созданные инциденты в базе данных.
Проверяет события на соответствие заранее определенным шаблонам для создания инцидентов.

Шаблоны для создания инцидентов:

Шаблон 1 (простой): Если получено событие с Event.Type = 1, то создать инцидент типа 1.

Шаблон 2 (составной): Если получено событие с Event.Type = 2, а затем в течение 20 секунд (и не позже!) получено событие с Event.Type = 1, то создать инцидент с Incident.Type = 2. В противном случае — создать инцидент Incident.Type = 1 на основе события с Event.Type = 2.

Шаблон 3 (составной): Если получено событие с Event.Type = 3, а затем в течение 60 секунд (и не позже!) создается инцидент с Incident.Type = 2, то создать инцидент с Incident.Type = 3. В противном случае — создать инцидент Incident.Type = 1 на основе события с Event.Type = 3.
