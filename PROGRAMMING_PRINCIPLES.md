# Принципи програмування

Початково, WPF проект диспетчера задач мав значні порушення головних принципів програмування. Три головні проблеми були в порушенні принципів інверсії залежностей, одної відповідальності та DRY (не повторюйся). Було зроблено успішно рефакторинг проекту, після якого в проект застосовано дані принципи:

### DRY - Don't repeat yourself

DRY (Don't repeat yourself) - це принцип, за яким треба уникати повторення коду, який може змінюватися в майбутньому.

Головне порушення було в створенні [кількох команд](ProcessManagerWpfApp\ViewModels\MainViewModel.cs#L55) для відкривання програм, які відрізнялися лише в тому, які вони файли запускали. Також у [MainWindow.xaml](ProcessManagerWpfApp\Views\MainWindow.xaml#L13) створювалися вручну кнопки для виклику даних програм. Якщо б ми хотіли змінити якось логіку запуску, то ми б мали міняти кожну команду.

Тепер в проєкті:
- Замість окремих команд для кожної програми зроблена одна команда [StartCommand](ProcessManagerWpfApp\ViewModels\MainViewModel.cs#L44), яка запускає процес залежно від [параметра](ProcessManagerWpfApp\ViewModels\MainViewModel.cs#L57), який передається через [RelayCommand](ProcessManagerWpfApp\ViewModels\RelayCommand.cs) в [IProcessManager](ProcessManagerWpfApp\Interfaces\IProcessManager.cs).
- Щоб не писати вручну кнопки і не міняти кожну кнопку при одній зміні, в MainViewModel є властивість [ProgramButtonInfos](ProcessManagerWpfApp\ViewModels\MainViewModel.cs#L17), яка є списком об'єктів [ProgramButtonInfo](ProcessManagerWpfApp\Models\ProgramButtonInfo.cs), які описують інформацію про назву кнопки та шлях до файлу.
- Також для пріоритетів процесів є властивість [PriorityList](ProcessManagerWpfApp\ViewModels\MainViewModel.cs#L17), яка потрібна щоб при можливій змінні назв пріоритетів не треба би було міняти по всьому коду назви.
- В [MainWindow.xaml](ProcessManagerWpfApp\Views\MainWindow.xaml#L13) тепер використовується UI елемент ItemsControl, в якому є один [шаблон кнопки](ProcessManagerWpfApp\Views\MainWindow.xaml#L22). В них вказуються властивості та команди з MainViewModel.
- [UI елемент ComboBox](ProcessManagerWpfApp\Views\MainWindow.xaml#L54) тепер приймає PriorityList в свою властивість ItemsSource

## SOLID

### Single Responsibility Principle

Принцип єдиної відповідальності (англ. Single Responsibility Principle) - це важливий принцип об'єктно-орієнтованого програмування, який побудований на тому, що клас має виконувати лише одну задачу, і як наслідок лише одну причину для змін.

Головне порушення даного принципа було в класі [MainWindow.xaml.cs](ProcessManagerWpfApp\Views\MainWindow.xaml.cs), який був "божественним об'єктом", який виконував відображення даних:
- Додає об'єкти інформації про процеси в [processesDataGrid](ProcessManagerWpfApp/MainWindow.xaml.cs#L31)
- Відображає MessageBox для користувача, як в даному сценарії
Але водночас і відповідав за бізнес-логіку програми:
- Весь клас MainWindow працює з WindowsProcessManager та WindowsProcessProvider, що змішує логіку програми з інтерфейсом.

Тепер проект слідує цьому принципу:
- Клас [MainViewModel](ProcessManagerWpfApp\ViewModels\MainViewModel.cs) є посередником, який перетворює сирі дані з класів [бізнес-логіки](ProcessManagerWpfApp\Models) для використання в [інтерфейсі](ProcessManagerWpfApp\Views\MainWindow.xaml)
- Інтерфейси [IProcessManager](ProcessManagerWpfApp\Interfaces\IProcessManager.cs) та [IProcessProvider](ProcessManagerWpfApp\Interfaces\IProcessProvider.cs) є окремими, оскільки IProcessManager відповідає за керування процесами, в той час як IProcessProvider виконує іншу відповідальність надання процесів.
- Діалогові вікна виводяться через [IDialogService](D:\momind\Dev\Labs\SWС\SWCLab1\ProcessManagerWpfApp\Interfaces\IDialogService.cs). Таким чином, [ViewModel не використовує MessageBox](ProcessManagerWpfApp\ViewModels\MainViewModel.cs#L50), який є графічним об'єктом.
- [Команди](ProcessManagerWpfApp\ViewModels\MainViewModel.cs#L54) у MainViewModel є засобом виконання дії для інтерфейса в XAML, при якому інтерфейс не викликатиме напряму методи з IProcessManager та IProcessProvider. [Методи самого MainViewModel](ProcessManagerWpfApp\ViewModels\MainViewModel.cs#L80) виконують методи бізнес-логіки, та готують їх для інтерфейсу. Самі команди крім виконання методів з MainViewModel [виконують оновлення процесів через LoadProcesses()](ProcessManagerWpfApp\ViewModels\MainViewModel.cs#L54). Таким чином методи MainViewModel класа мають одну відповідальність не змішану з оновленням UI.
- Сам інтерфейс пов'язується з MainViewModel через DataContext. В XAML файлі реалізовано механізм прив'язки даних до відповідних властивостей та команд MainViewModel. Таким чином, інтерфейс лише використовує готові дані, які MainViewModel утворює з класів в [Model](ProcessManagerWpfApp\Models)

### Interface Segregation Principle

Принцип розподілення інтерфейу (англ. Interface Segregation Principle) - це один з принципів SOLID, який полягає в тому, що клієнти не повинні реалізувати методи інтерфейса, які вони не використовують.

Початковий клас ProcessManager був повним класом, який відповідав за отримання та керування процесами. Для дотримання принципу розділення інтерфейсу було утворено два окремі інтерфейси IProcessProvider та IProcessManager, а не один зі всіма методами.
Таким чином, якщо б ми хотіли утворити наприклад програму, яка лише моніторить процеси, нам не потрібно реалізувати не потрібні для неї методи встановлення пріоритету, закривання та запуску процеса. Замість цього клас реалізував би інтерфейс IProcessProvider, який лише отримує процеси.
Але оскільки наша програма є повноцінним диспетчером задач, вона також [реалізує IProcessManager](ProcessManagerWpfApp\Models\WindowsProcessManager.cs), через який вона додатково керує процесами.

### Dependency Inversion Principle

Принцип інверсії залежностей (англ. Dependency Inversion Principle) - це важливий принцип, який полягає в розриві жорсткого зв'язку програмних модулів вищого та нижнього рівня через спільні інтерфейси.

Проблема була в тому, що MainWindow.xaml.cs напряму використовував клас ProcessManager, що робило жорстке прив'язання до конкретної реалізації менеджера процесів під Windows. Це не дало би нам мати іншу реалізацію програми для іншої ОС, для якої робота з процесами відрізняється. Також напряму використовувати MessageBox для діалогових вікон змусило б нас міняти кожен рядок з викликом цього класа при портуванні на іншу ОС.

Тепер в нашому проєкті:
- Клас приймає залежності за інтерфейсами [IDialogService](ProcessManagerWpfApp\Interfaces\IDialogService.cs), [IProcessManager](ProcessManagerWpfApp\Interfaces\IProcessManager.cs) та [IProcessProvider](ProcessManagerWpfApp\Interfaces\IProcessProvider.cs). Таким чином, [ViewModel клас](ProcessManagerWpfApp\ViewModels\MainViewModel.cs) не залежить напряму від реалізацій для Windows, і в майбутньому може працювати й з іншими реалізаціями (наприклад на Linux і macOS).
- [В MainWindow.xaml.cs](ProcessManagerWpfApp\Views\MainWindow.xaml.cs#L17) передаються класи [WindowsProcessManager](ProcessManagerWpfApp\Models\WindowsProcessManager.cs) та [WindowsProcessProvider](ProcessManagerWpfApp\Models\WindowsProcessProvider.cs), які є реалізаціями IProcessManager та IProcessProvider відповідно для процесів саме у Windows.
- В тому самому класі передається [WindowsDialogService](ProcessManagerWpfApp\Views\WindowsDialogService.cs), який реалізує інтерфейс сервіса діалогів через MessageBox клас під Windows.