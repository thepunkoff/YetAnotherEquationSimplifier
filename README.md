# Yet Another Equation Simplifier

Тестовое задание для компании "Антиплагиат"

## Представляю вам мой парсер и по совместительству упроститель линейных уравнений!

Приложение состоит по меньшей мере из 2х частей: **парсера** и **упростителя**. Парсер идет по строке ввода и токенизирует
следующий(ие) символ(ы), а упроститель совершает действия в зависимости от токена.
В парсере используется **shunting yard algorithm**. Этот алгоритм с помощью друх стеков умеет переводить выражение и инфиксной нотации в обратную польскую (постфиксную) нотацию.
Так же с помощью него можно создавать абстрактные синтаксические деревья. Ради простоты, я решил не токенизировать всё уравнение сразу и потом производить упрощение,
а наоборот — при помощи этого алгоритма решил упрощать выражение по ходу действия. Когда упрощать, а когда нет — говорит алгоритм, а вот само упрощение я реализовал сам.
На самом деле я не пытался повторить алгоритм целиком из источников, а я прочитал, как он работает и больше не читал, так что реализация может хромать.
Но это был очень сложный и интересный челлендж для меня, так что я не подсматривал! =)

Парсер знает о 5 видах **токенов**: константа, переменная, дробь, выражение и бинарный знак.
Упроститель умеет производить 5 основных **действий**: сложение, вычитание, умножение, деление, возведение в степень — каждого из этих токенов с каждым.
Также приложение сокращает дроби и выстраивать части выражения в нужном порядке.
Парсер проверяет синтаксис на основе предыдущего символа. Если эти символы не могут стоять вместе — он возвращает ошибку синтаксиса.
Если какое либо из действий совершить не удается, возвращается причина неудачи.

Задание я выполнял целую неделю и потратил 40+ часов. За это время я:
- Познакомился c shunting yard algorithm, recursive descent algorithm
- Похнакомился с теорей формальных языков
- Поработал с precedence и associativity математических операторов
- Реализовал различные математические принципы и алгебраические формулы
- Поработал с наследованием, чего раньше делал крайне мало
- Придумал много, очень много костылей
- Обнаружил много полезных методов и функций в библиотеках C# (мой любимый LINQ метод теперь Enumerable.Aggregate =))
- Понял, как важен TDD подход в реализации вещей, которые могут сломаться от одного дуновения, да и для любых проектов.
- Поймал баг, связанный с изменением состояния объекта в методе ToString(), больше не меняю состояние в ToString().
    кстати, невероятно удивительно было обнаружить, что все работало только если я наводил курсор на переменную в дебаг-режиме =)
- Определенно захотел дальше изучить конечные автоматы и написать этот парсер еще раз, но уже прибегая к более продвинутым технологиям.
---
Подробности реализации, баги и другие неоптимальные, но работающие решения ищите внутри)
Буду рад послушать, что хорошо, что плохо, и ответить на любые вопросы!