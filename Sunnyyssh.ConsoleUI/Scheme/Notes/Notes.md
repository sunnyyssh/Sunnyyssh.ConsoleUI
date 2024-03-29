﻿1. It's faster to write all in single Write call than split it in different Write calls.
2. You can make manager have different drawing options: redraw the whole console or only changed element.
3. Maybe it's better to make manager draw all than draw them separately.
4. It's faster to rewrite the whole string ^-^
5. To set cursor position is not so slow. (No, actually it is).
6. How to optimize colorized text.
7. Container must not contain itself.

Требования к реализации отрисовки:
1. Тип, являющийся наследником UIElement, должен быть неспособен сломать логику отрисовки
2. Определение отрисовки не должно лежать полностью на переопределении методов

[//]: # (3. Наследник UIElement не должен сам ничего рисовать)
[//]: # (4. Работа непосредственно с консолью должна быть инкапсулированна в единственном классе-менеджере)

Требования к внутренней реализации отрисовки:
1. Можно хранить прошлый стейт на всякий случай.

[//]: # (1. Количество вызовов методов класса Console должно быть минимально.)
[//]: # (2. Перерисовывать два куска одной строки в одном вызове Console.WriteLine когда это возможно.)
[//]: # (4. Можно сделать коллекцию запросов отрисовки, в которую будут добавляться запросы, если не закончена еще прошлая отрисовка)

Как организовать focus flow:
1. IFocusable не должен иметь объект его focus менеджера
2. Сторонний код не должен иметь возможность влиять на flow
3. Весь focus flow должен быть инкапсулирован внутри Wrapper и UIManager
4. Легче всего сделать на событиях, но события не могут быть инкапсулированы в интерфейсах

Какие элементы интерфейса должны быть реализованы:
1. Меню
2. Кнопка
3. Текстовый блок
4. Рамка
5. Табличка

Перед релизом: 
1. Проставь атрибуты DebuggerStepThrough там, где необходимо.
2. Задокументируй все xml
3. Проверь на потокобезопасность все

Общие замечания:
1. Реализуй потокобезопасность
2. Сделай события завершения и начала UIManager.Run
3. После реализации ядра перерисуй его uml-диаграмму
4. Вызовы публичных методов не должны влечь за собой отрисовку в данном потоке

Требования к потокам:
1. Вызывающий код не должен застаивать при вызове методов
2. Работа с консолью должна быть не в вызывающем потоке
3. Многопоточность должна быть инкапсулирована в менеджере
4. Потоки работы:
    - Обработка нажатий клавиш
    - Отрисовка
    - Жизнь объектов вне физического их представления

Менеджерские задачи надо разбить:
1. Менеджер клавиш
2. Менеджер отрисовки
3. Менеджер фокусфлоу

- Разбить код большого класса на несколько файлов?
- Надо подчищать состояние после завершения работы 
- ОБЯЗАТЕЛЬНО ОПТИМИЗИРУЙ ОТРИСОВКУ