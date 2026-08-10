<p align="center">
  <a href="./README.md">
    <img src="https://img.shields.io/badge/English-0078D4?style=for-the-badge" alt="English">
  </a>
  <a href="./README.ru.md">
    <img src="https://img.shields.io/badge/Русский-D52B1E?style=for-the-badge" alt="Русский">
  </a>
</p>

# PinWindow — окна поверх остальных

[![Последний релиз](https://img.shields.io/github/v/release/helliong/pinWindow?style=flat-square&label=релиз)](https://github.com/helliong/pinWindow/releases/latest) [![Windows](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D4?style=flat-square&logo=windows)](https://github.com/helliong/pinWindow)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/) [![Author](https://img.shields.io/badge/Author-helliong-181717?style=flat-square&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA5MSA2NSI%2BPHBhdGggZmlsbD0iI2ZmZiIgZmlsbC1ydWxlPSJldmVub2RkIiBkPSJNMCwwIDQ2LDAgNTIsMTMgNTQsMTQgNTQsMTYgNTYsMTcgNTYsMTkgNTgsMjAgNjEsMjYgNjMsMjUgNjQsMjEgNjYsMjAgNjcsMTYgNjksMTUgNzEsOSA3Myw4IDc3LDAgOTEsMCA5MSwxIDg4LDQgODcsOCA4NSw5IDgzLDE1IDgxLDE2IDgwLDIwIDc4LDIxIDc3LDI1IDc1LDI2IDczLDMyIDcxLDMzIDcwLDM3IDY4LDM4IDY2LDQ0IDY0LDQ1IDYyLDUxIDYwLDUyIDU5LDU2IDU3LDU3IDUzLDY1IDAsNjVaTTEzLDEyIDEzLDI1IDM4LDI1IDM4LDM3IDEzLDM3IDEzLDUzIDQ2LDUzIDUwLDQ1IDUyLDQ0IDU0LDM3IDQ5LDMxIDQzLDE5IDQxLDE4IDQwLDE0IDM4LDEyWiIvPjwvc3ZnPg%3D%3D)](https://helliong.space)

<!-- [![Загрузки SourceForge](https://img.shields.io/sourceforge/dm/pinwindow?style=flat-square&label=загрузки)](https://sourceforge.net/projects/pinwindow/files/) -->

> Закрепляйте любое окно поверх остальных одним нажатием.

[![Скачать PinWindow](https://a.fsdn.com/con/app/sf-download-button)](https://sourceforge.net/projects/pinwindow/files/latest/download)

PinWindow — лёгкая утилита для Windows, которая позволяет закреплять окна поверх всех остальных приложений.

Закрепить или открепить активное окно можно с помощью кнопки рядом с его заголовком, настраиваемого глобального сочетания клавиш или меню в системном трее.

## Возможности

- Закрепление поддерживаемых окон поверх остальных
- Кнопка с булавкой рядом с заголовком активного окна
- Настраиваемое глобальное сочетание клавиш
- Управление активным окном через системный трей
- Настройка размера и активного цвета кнопки
- Изменение горизонтального и вертикального положения кнопки
- Автоматический запуск вместе с Windows
- Русский и английский языки интерфейса
- Автоматическая проверка обновлений
- Отдельное окно уведомления о новой версии
- Ссылки на GitHub и создание сообщения об ошибке в настройках
- Системные уведомления
- Работа в фоновом режиме
- Установочная и переносимая версии
- Поддержка Windows 10 и Windows 11
- Переносимая версия не требует отдельной установки .NET

## Скачать

Последнюю версию PinWindow можно скачать здесь:

- [GitHub Releases](https://github.com/helliong/pinWindow/releases/latest)
- [SourceForge](https://sourceforge.net/projects/pinwindow/files/)

Доступные файлы:

| Файл                                    | Описание                      |    Размер |
| --------------------------------------- | ----------------------------- | --------: |
| `PinWindowSetup-v3.3.3-win-x64.exe`     | Компактный установщик         |  ~2,07 МБ |
| `PinWindow-v3.3.3-portable-win-x64.exe` | Автономная переносимая версия | ~68,33 МБ |
| `SHA256SUMS.txt`                        | Контрольные суммы SHA-256     |         — |

Для большинства пользователей рекомендуется версия с установщиком.

## Установка

### Установщик

1. Скачайте `PinWindowSetup-v3.3.3-win-x64.exe`.
2. Запустите установщик.
3. Выберите папку установки и дополнительные параметры.
4. Завершите установку и запустите PinWindow.

Если .NET 8 Desktop Runtime отсутствует, установщик загрузит его с серверов Microsoft. Для этого может потребоваться подключение к интернету.

### Переносимая версия

1. Скачайте `PinWindow-v3.3.3-portable-win-x64.exe`.
2. Переместите файл в любую удобную папку.
3. Запустите приложение.

Переносимая версия содержит необходимые компоненты .NET и не требует установки.

Microsoft Defender SmartScreen может показать предупреждение, поскольку приложение пока не имеет цифровой подписи. Запускайте только файлы, скачанные из официального репозитория GitHub или со страницы проекта на SourceForge.

## Использование

### Кнопка возле заголовка окна

1. Откройте приложение, окно которого необходимо закрепить.
2. Сделайте это окно активным.
3. Нажмите кнопку с булавкой рядом с заголовком окна.
4. Нажмите кнопку ещё раз, чтобы открепить окно.

Закреплённое окно будет оставаться поверх остальных окон приложений.

### Сочетание клавиш

Сделайте нужное окно активным и нажмите настроенное глобальное сочетание клавиш.

Сочетание клавиш по умолчанию:

```text
Ctrl + Alt + T
```

Сочетание клавиш можно изменить в настройках PinWindow.

### Системный трей

Нажмите правой кнопкой мыши по значку PinWindow в системном трее, чтобы:

- Закрепить или открепить активное окно
- Открыть настройки
- Показать или скрыть кнопку возле заголовка окна
- Посмотреть текущее сочетание клавиш
- Закрыть PinWindow

Дважды нажмите по значку в трее, чтобы открыть окно настроек.

## Настройки

В PinWindow можно настроить:

- Язык интерфейса: русский или английский
- Глобальное сочетание клавиш
- Отображение кнопки закрепления
- Размер кнопки
- Активный цвет кнопки
- Горизонтальное и вертикальное смещение кнопки
- Системные уведомления
- Автоматический запуск вместе с Windows

В окне настроек также отображается установленная версия PinWindow и находятся ссылки на репозиторий GitHub и страницу создания сообщения об ошибке.

Настройки сохраняются отдельно для текущего пользователя Windows:

```text
%AppData%\PinWindow\settings.json
```

## Уведомления об обновлениях

PinWindow автоматически проверяет наличие новой версии на GitHub.

Если обновление найдено, приложение показывает отдельное окно с установленной и актуальной версиями. Пользователь может открыть страницу загрузки релиза или отложить обновление.

PinWindow не устанавливает обновления автоматически.

## Переносимая версия

Переносимая версия распространяется в виде одного автономного исполняемого файла. Её можно запускать из любой папки или с USB-накопителя без установки.

Настройки при этом сохраняются отдельно в профиле текущего пользователя Windows.

## Системные требования

- Windows 10 или Windows 11
- 64-разрядная версия Windows
- Подключение к интернету для проверки обновлений
- Установщику может потребоваться интернет для загрузки .NET 8 Desktop Runtime
- Переносимая версия не требует отдельной установки .NET

## Текущая версия

### PinWindow v3.3.3

Изменения в этой версии:

- Исправлено исчезновение кнопки закрепления при переключении между приложениями и окнами Проводника
- Восстановлена работа кнопки после использования приложения «Ножницы» или закрытия Telegram
- Исправлено зависание значка закрепления на рабочем столе после закрытия целевого окна
- Исправлено неправильное положение значка при переключении и перемещении окон
- Добавлены русский и английский языки интерфейса
- В настройки добавлены версия приложения, ссылки на GitHub и создание сообщения об ошибке
- Добавлена автоматическая проверка обновлений и отдельное окно уведомления
- Улучшены стабильность и производительность отображения кнопки закрепления

## Файлы релиза

### Установщик

```text
PinWindowSetup-v3.3.3-win-x64.exe
```

Компактный установщик. Если .NET 8 Desktop Runtime отсутствует, он будет загружен с серверов Microsoft.

### Переносимая версия

```text
PinWindow-v3.3.3-portable-win-x64.exe
```

Автономная переносимая версия, которая не требует установки приложения или отдельной установки .NET.

## Контрольные суммы SHA-256

### Установщик

```text
D5596EC14E42AB24E26E151D23035F56C792747D2AD426444738AF106BAAACB2
```

### Переносимая версия

```text
4022EC6323DCC6748B4A6FF35ECDBB2FA76F511AA8E124893598DBA3836AD14B
```

## Проверка скачанных файлов

Откройте PowerShell в папке со скачанными файлами.

Проверка установщика:

```powershell
Get-FileHash ".\PinWindowSetup-v3.3.3-win-x64.exe" -Algorithm SHA256
```

Проверка переносимой версии:

```powershell
Get-FileHash ".\PinWindow-v3.3.3-portable-win-x64.exe" -Algorithm SHA256
```

Полученные значения должны совпадать с опубликованными выше контрольными суммами и содержимым файла `SHA256SUMS.txt`.

## Исходный код

Полный исходный код доступен на GitHub:

[https://github.com/helliong/pinWindow](https://github.com/helliong/pinWindow)

## Сообщения об ошибках и предложения

Сообщить об ошибке или предложить новую функцию можно через GitHub Issues:

[https://github.com/helliong/pinWindow/issues](https://github.com/helliong/pinWindow/issues)

При создании сообщения об ошибке желательно указать:

- Версию Windows
- Версию PinWindow
- Название приложения, в котором возникла проблема
- Последовательность действий для воспроизведения ошибки
- Полный текст ошибки
- Снимок экрана или запись экрана, когда это необходимо

## Сборка из исходного кода

### Требования

- Windows 10 или Windows 11
- Git
- .NET 8 SDK
- Inno Setup 7 для сборки установщика

Клонируйте репозиторий:

```powershell
git clone https://github.com/helliong/pinWindow.git
cd pinWindow
```

Запустите PinWindow в режиме разработки:

```powershell
dotnet run
```

Создайте релизную сборку:

```powershell
dotnet build -c Release
```

### Сборка установщика

```powershell
powershell -ExecutionPolicy Bypass `
    -File ".\scripts\publish-installer.ps1" `
    -Version 3.3.3
```

Готовый установщик будет создан в папке:

```text
artifacts\installer\
```

### Сборка переносимой версии

```powershell
dotnet publish `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o ".\artifacts\portable\publish"
```

Готовый исполняемый файл будет создан в папке:

```text
artifacts\portable\publish\
```

## Ссылки проекта

- [Проект на SourceForge](https://sourceforge.net/projects/pinwindow/)
- [Репозиторий GitHub](https://github.com/helliong/pinWindow)
- [Последний релиз на GitHub](https://github.com/helliong/pinWindow/releases/latest)
- [Сообщить об ошибке](https://github.com/helliong/pinWindow/issues)
- [Исходный код](https://github.com/helliong/pinWindow)

## Участие в разработке

Участие в развитии проекта приветствуется.

Вы можете помочь проекту следующими способами:

- Сообщить об ошибке
- Предложить новую функцию
- Улучшить документацию
- Отправить Pull Request

Перед отправкой Pull Request убедитесь, что проект успешно собирается:

```powershell
dotnet build
```

## Автор

Разработчик: [helliong](https://github.com/helliong).

---

<p align="center">
  <a href="./README.md">English</a>
  ·
  <a href="./README.ru.md">Русский</a>
</p>
