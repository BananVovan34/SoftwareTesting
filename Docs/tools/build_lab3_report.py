"""Build the Lab 3 report from the actual C# example output and MSTest TRX."""
from __future__ import annotations

import html
import json
import math
from pathlib import Path
from xml.etree import ElementTree as ET

from PIL import Image as PILImage, ImageDraw, ImageFont
from reportlab.graphics.shapes import Drawing, String, Line
from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (SimpleDocTemplate, Paragraph, Spacer, PageBreak,
                               Table, TableStyle, Image, Preformatted, KeepTogether)

ROOT = Path(__file__).resolve().parents[2]
DOCS = ROOT / "Docs"
ASSETS = DOCS / "images"
RESULTS = DOCS / "Results" / "2026-11-03"
OUTPUT = ROOT / "output" / "pdf" / "Отчет_ЛР3_Вариант_3.pdf"
EDGES = {(1, 2), (2, 3), (2, 7), (3, 4), (3, 5), (4, 2), (5, 6), (6, 2)}


def number(value):
    return f"{value:.12g}".replace(".", ",")


def path_text(path):
    return " → ".join(map(str, path))


def graph():
    im = PILImage.new("RGB", (1440, 1250), "white")
    draw = ImageDraw.Draw(im)
    font = ImageFont.truetype("C:/Windows/Fonts/arial.ttf", 44)
    label_font = ImageFont.truetype("C:/Windows/Fonts/arial.ttf", 39)
    def point(p):
        return (65 + (p[0] + 3.45) * 200, 50 + (5.35 - p[1]) * 200)
    svg = ['<svg xmlns="http://www.w3.org/2000/svg" width="1440" height="1250" viewBox="0 0 1440 1250">',
           '<rect width="1440" height="1250" fill="white"/>',
           '<defs><marker id="arrow" markerWidth="9" markerHeight="9" refX="8" refY="4.5" orient="auto"><path d="M0,0 L9,4.5 L0,9 Z" fill="black"/></marker></defs>']
    positions = {1: (0, 5), 2: (0, 4), 3: (0, 2.7), 4: (-1.4, 1.55),
                 5: (1.4, 1.55), 6: (1.4, .55), 7: (-2.7, 0)}
    routes = [
        [(0, 4.8), (0, 4.2)],
        [(0, 3.8), (0, 2.9)],
        [(-.2, 4), (-3.15, 4), (-3.15, 0), (-2.9, 0)],
        [(-.14, 2.55), (-1.25, 1.7)],
        [(.14, 2.55), (1.25, 1.7)],
        [(-1.6, 1.55), (-2.1, 1.55), (-2.1, 3.4), (-.15, 3.85)],
        [(1.4, 1.35), (1.4, .75)],
        [(1.6, .55), (2.6, .55), (2.6, 4), (.2, 4)],
    ]
    for route in routes:
        points = [point(p) for p in route]
        draw.line(points, fill="black", width=4, joint="curve")
        end, prev = points[-1], points[-2]
        angle = math.atan2(end[1] - prev[1], end[0] - prev[0])
        sides = [(end[0] - 22 * math.cos(angle + delta), end[1] - 22 * math.sin(angle + delta))
                 for delta in (-.4, .4)]
        draw.polygon([end] + sides, fill="black")
        svg.append('<polyline points="' + ' '.join(f'{x:.1f},{y:.1f}' for x, y in points) +
                   '" fill="none" stroke="black" stroke-width="4" marker-end="url(#arrow)"/>')
    for node, (x, y) in positions.items():
        px, py = point((x, y))
        if node in (2, 3):
            poly = [(px, py-40), (px+40, py), (px, py+40), (px-40, py)]
            draw.polygon(poly, fill="white", outline="black", width=4)
            svg.append('<polygon points="' + ' '.join(f'{a},{b}' for a, b in poly) + '" fill="white" stroke="black" stroke-width="4"/>')
        else:
            draw.ellipse((px-40, py-40, px+40, py+40), fill="white", outline="black", width=4)
            svg.append(f'<circle cx="{px}" cy="{py}" r="40" fill="white" stroke="black" stroke-width="4"/>')
        draw.text((px, py), str(node), font=font, fill="black", anchor="mm")
        svg.append(f'<text x="{px}" y="{py}" font-family="Arial" font-size="44" text-anchor="middle" dominant-baseline="central">{node}</text>')
    for x, y, text in [(0.22, 3.4, "Да"), (-1.15, 4.15, "Нет"),
                       (-.95, 2.4, "Да"), (.93, 2.4, "Нет"),
                       (-1.05, 3.1, "R1"), (1.25, 3.1, "R2"), (-2.8, 4.8, "R3")]:
        px, py = point((x, y))
        draw.text((px, py), text, font=label_font, fill="black", anchor="mm")
        svg.append(f'<text x="{px}" y="{py}" font-family="Arial" font-size="39" text-anchor="middle" dominant-baseline="central">{text}</text>')
    im.save(ASSETS / "lab3-flow.png")
    (ASSETS / "lab3-flow.svg").write_text('\n'.join(svg) + '\n</svg>\n', encoding="utf-8")


def equation():
    drawing = Drawing(310, 79)
    drawing.hAlign = "CENTER"
    def text(x, y, value, size):
        drawing.add(String(x, y, value, fontName="LabTimes", fontSize=size))
    text(8, 27, "L", 23)
    text(22, 21, "k", 12)
    text(31, 27, "(t) =", 23)
    text(99, 19, "∏", 39)
    text(110, 62, "n", 13)
    text(86, 1, "i = 0, i ≠ k", 11)
    drawing.add(Line(155, 35, 257, 35, strokeColor=colors.black, strokeWidth=.8))
    text(179, 45, "t - x", 21)
    text(218, 40, "i", 12)
    text(171, 10, "x", 21)
    text(183, 5, "k", 12)
    text(195, 10, "- x", 21)
    text(222, 5, "i", 12)
    return drawing


def build_pages(data, counters, lab_count):
    cases = {row["Id"]: row for row in data["Results"]}
    functional = [row for row in data["Results"] if row["InDomain"]]
    expected_paths = {"S01": [1, 2, 7], "T01": [1, 2, 3, 4, 2, 7],
                      "T02": [1, 2, 3, 5, 6, 2, 3, 4, 2, 7]}
    for case_id, expected in expected_paths.items():
        assert cases[case_id]["Path"] == expected, case_id
    covered = {edge for row in functional for edge in zip(row["Path"], row["Path"][1:])}
    assert covered == EDGES
    assert all(row["Passed"] for row in data["Results"])
    assert counters["failed"] == "0" and counters["passed"] == counters["total"]

    code = """L = 1.0;                     // вершина 1
i = 0;
while (i <= n) {              // вершина 2
    if (i == k) {            // вершина 3
        ++i;                 // вершина 4
        continue;
    }
    L *= (t - x[i]) / (x[k] - x[i]); // вершина 5
    ++i;                     // вершина 6
}
// Ответ L                   // вершина 7"""
    p1 = [
        ("title", "Структурное тестирование многочлена Лагранжа"),
        ("subtitle", "Лабораторная работа 3 Вариант 3"),
        ("p", "Дата выполнения: 03.11.2026. Тема: структурное тестирование и тестирование белого ящика."),
        ("h", "Цель и результат работы"),
        ("p", "Изучить тестирование базового пути на фрагменте вычисления элементарного интерполяционного многочлена. Построен потоковый граф из семи вершин и восьми дуг. Его цикломатическая сложность равна 3. Подготовлены три независимых пути графа, восемь примеров на допустимых данных и отдельный структурный эксперимент."),
        ("p", f"Все {lab_count} автоматические проверки ЛР3 прошли. Вместе с проверками калькулятора выполнено {counters['total']} тестов, ошибок и пропусков нет. Для допустимых примеров подтверждены все восемь дуг графа. Путь с нулём итераций отмечен как недостижимый в математической области входных данных."),
        ("h", "Математическая постановка"),
        ("p", "Даны n + 1 различных узлов x₀, …, xₙ, индекс k и точка t. Требуется вычислить один элементарный многочлен Lₖ(t). Значения функции в узлах не нужны: весь интерполяционный многочлен здесь не строится."),
        ("formula", r"L_k(t)=\prod_{i=0,\ i\ne k}^{n}\frac{t-x_i}{x_k-x_i}"),
        ("p", "Допустимые данные: n ≥ 0, 0 ≤ k ≤ n; узлы попарно различны. В программе используются конечные значения типа double. Пустой массив, неверный индекс, повторяющиеся узлы и NaN или бесконечность отклоняются до вычисления. При n = 0 произведение не содержит множителей и равно 1."),
        ("h", "Краткие теоретические сведения"),
        ("p", "При тестировании белого ящика тесты выбирают с учётом исходного кода. Покрытие операторов требует выполнить каждый оператор, покрытие решений - каждый исход условия, покрытие условий - оба значения простых условий. Покрытие решений и условий объединяет эти требования. Комбинаторное покрытие проверяет возможные сочетания значений условий."),
        ("p", "Метод базового пути использует граф управления. Вершина представляет линейный участок кода или условие, дуга - возможный переход. Независимый путь добавляет дугу, отсутствующую в ранее выбранных путях. Цикломатическая сложность задаёт размер базиса графа; достижимость его путей проверяется по значениям переменных."),
    ]
    p2 = [
        ("h", "Исходный алгоритм и нумерация"),
        ("p", "Сохранены цикл, проверка i == k, переход continue и порядок перемножения из варианта 3 методички. В листинге опущены ввод, проверка предусловий и регистрация маршрута."),
        ("code", code),
        ("table", (["Вершина", "Действие или условие"], [
            ["1", "L = 1.0; i = 0 - начальные значения"],
            ["2", "i <= n - условие продолжения цикла"],
            ["3", "i == k - требуется пропустить k-й множитель"],
            ["4", "++i; continue - перейти к следующей итерации"],
            ["5", "L *= (t - x[i]) / (x[k] - x[i])"],
            ["6", "++i после вычисления множителя"],
            ["7", "Завершение вычисления и возврат L"],
        ], [55, 450])),
        ("h", "Границы структурного анализа"),
        ("p", "Метрика и покрытие относятся к вычислительному фрагменту EvaluateFragment в файле Lab3/LagrangeBasis.cs. Оболочка Calculate проверяет входные данные, а консольная программа отвечает за ввод и вывод; эти участки не включены в граф. Их ошибки проверены отдельными тестами."),
        ("p", "Для наблюдения за маршрутом функция Check записывает номер проверяемой вершины и возвращает исходное значение условия. Записи Path не меняют вычислительный алгоритм. Вершина 2 регистрируется и при последней, ложной проверке цикла. Вершина 4 ведёт прямо к вершине 2, поэтому после continue умножение и вершина 6 пропускаются."),
    ]
    p3 = [
        ("h", "Потоковый граф и цикломатическая сложность"),
        ("image", "lab3-flow.png"),
        ("p", "Рисунок 1. Потоковый граф вычислительного фрагмента. Ромбы 2 и 3 обозначают предикатные вершины. R1 и R2 - внутренние области, R3 - внешняя область."),
        ("p", "Дуги: 1 → 2; 2 → 3; 2 → 7; 3 → 4; 3 → 5; 4 → 2; 5 → 6; 6 → 2. Граф связный, имеет один вход и один выход."),
        ("table", (["Способ расчёта", "Расчёт", "Результат"], [
            ["По дугам и вершинам", "V(G) = E - N + 2 = 8 - 7 + 2", "3"],
            ["По предикатным вершинам", "V(G) = P + 1 = 2 + 1", "3"],
            ["По областям плоского графа", "R1 + R2 + R3", "3"],
        ], [155, 280, 70])),
        ("p", "Все три способа дают V(G) = 3. Число означает размер структурного базиса, а не три возможных полных запуска: число маршрутов с разным количеством итераций зависит от n."),
    ]
    results_rows = [[row["Id"], "; ".join(number(x) for x in row["Nodes"]), str(row["K"]),
                     number(row["T"]), number(row["Expected"]), number(row["Actual"])]
                    for row in functional]
    p4 = [
        ("h", "Базовое множество независимых путей"),
        ("p", "P1: 1 → 2 → 7. Цикл не выполняется. После i = 0 условие i <= n ложно только при n < 0. В допустимой области этот путь недостижим. Эксперимент S01 вызывает только исходный внутренний фрагмент с n = -1 и получает L = 1; это не корректная задача интерполяции."),
        ("p", "P2: 1 → 2 → 3 → 4 → 2 → 7. Данные T01: x = [3], n = 0, k = 0, t = 100. Единственный множитель пропускается, L = 1. Новые дуги относительно P1: 2 → 3, 3 → 4 и 4 → 2."),
        ("p", "P3: 1 → 2 → 3 → 5 → 6 → 2 → 3 → 4 → 2 → 7. Данные T02: x = [0; 2], n = 1, k = 1, t = 1. Сначала вычисляется множитель 1/2, затем пропускается индекс k. Новые дуги: 3 → 5, 5 → 6 и 6 → 2."),
        ("p", "Каждый следующий путь добавляет новые дуги, поэтому пути независимы. Их число равно V(G). На допустимых данных невозможно воспроизвести P1, но T02 уже покрывает все дуги и оба исхода каждого из двух условий. Покрытие дуг не требует пропускать цикл при входе."),
        ("h", "Тестовые данные и фактические результаты"),
        ("p", "Здесь n равен числу узлов минус один. Результаты получены запуском реализации C#; сравнение выполняется с абсолютным допуском 10⁻¹². Все восемь примеров прошли."),
        ("table", (["Тест", "Узлы x", "k", "t", "Ожидалось", "Получено"],
                   results_rows, [48, 143, 30, 62, 111, 111])),
        ("p", f"Отдельно S01: n = -1, x = [], k = 0, t = 0. Ожидалось 1; получено {number(cases['S01']['Actual'])}. Зарегистрирован путь {path_text(cases['S01']['Path'])}. Публичный метод Calculate отклоняет пустой массив, что подтверждено тестом."),
    ]
    p5 = [
        ("h", "Разбор вычисления и проверка покрытия"),
        ("p", "В T04 заданы x = [0; 1; 2], k = 1, t = 1,5. При i = 0 получаем множитель (1,5 - 0)/(1 - 0) = 1,5. При i = 1 выполняется continue. При i = 2 множитель равен (1,5 - 2)/(1 - 2) = 0,5. Итог: 1,5 × 0,5 = 0,75."),
        ("p", "Фактический маршрут T04: " + path_text(cases["T04"]["Path"]) + "."),
        ("p", "T03 проверяет пропуск первого индекса, T05 - значение 1 в собственном узле, T06 - значение 0 в другом узле. T07 проверяет экстраполяцию и отрицательный результат, T08 - цикл с четырьмя узлами. Дополнительный тест проверяет тождество суммы базисных многочленов: сумма Lₖ(t) по всем k равна 1."),
        ("p", "Покрытие рассчитывается по фактически зарегистрированным соседним вершинам Path, а не по списку заранее ожидаемых переходов. В допустимых примерах обнаружено 8 из 8 дуг и 4 из 4 исходов условий. Это покрытие выбранной модели; оно не является измерением покрытия всей сборки и не доказывает отсутствие ошибок."),
        ("h", "Результат автоматического прогона"),
        ("p", f"MSTest: {counters['passed']} из {counters['total']} тестов прошли. Из них {lab_count} проверки ЛР3: 8 примеров, 4 проверки маршрутов, 1 проверка покрытия, 1 проверка математического тождества и 8 проверок ошибок входа или диапазона. Ещё 43 теста относятся к калькулятору."),
        ("p", "Проверены отклонение null и пустого массива, двух недопустимых индексов, повторяющихся узлов, NaN в точке t, бесконечного узла и результата вне диапазона double. Для обычных данных точность ограничена арифметикой double; близкие узлы и экстремальные значения могут увеличивать численную ошибку."),
        ("h", "Запуск из директории решения"),
        ("code", "dotnet run --project Lab3/SoftwareTesting.Lab3.csproj\n# Ввести узлы через пробел, затем k и t\n\n# Пример с аргументами: k=1, t=1, x=[0; 2]\ndotnet run --project Lab3/SoftwareTesting.Lab3.csproj -- 1 1 0 2\n\n# Все тесты решения\ndotnet test SoftwareTesting.sln"),
        ("p", "В Visual Studio выбрать проект SoftwareTesting.Lab3 в обозревателе решения и назначить его запускаемым. Для вывода сохранённых примеров использовать аргумент --demo. Полные результаты находятся в Docs/Results/2026-11-03/lab3-cases.json и lab3.trx."),
        ("p", "Дополнительно прошли семь проверок консольного запуска: аргументы, дробь с запятой, ввод по запросам, повторяющиеся узлы, неверный индекс, некорректное число и справка. Ввод, вывод и коды возврата сохранены в lab3-cli.json рядом с остальными результатами."),
    ]
    questions = [
        ("1 Понятие тестирования", "Тестирование - процесс проверки программного обеспечения, включающий подготовку и выполнение проверок, оценку результатов и выявление расхождений с ожидаемым поведением. Успешный прогон выбранных тестов не доказывает отсутствие всех дефектов."),
        ("2 Тест и процесс тестирования", "Тест задаёт входные данные, предусловия, действия и ожидаемый результат. Процесс включает анализ объекта, выбор проверок, подготовку среды, выполнение, сравнение результатов, регистрацию дефектов и повторную проверку исправлений."),
        ("3 Структурное тестирование", "Это тестирование, при котором сценарии выбирают по внутренней структуре программы. Для управления потоком используют граф, условия, ветви и циклы; полноту оценивают по выбранному критерию покрытия."),
        ("4 Суть тестирования белого ящика", "Тестировщик знает исходный код и подбирает данные для выполнения конкретных операторов и переходов. Проверяются внутренние элементы и их взаимодействие, а результаты сопоставляются с ожидаемыми."),
        ("5 Особенности белого ящика", "Проверяют независимые пути, оба исхода решений и допустимые режимы циклов. При составных условиях учитывают порядок вычисления и короткое замыкание. Достижимость пути устанавливают по данным и состоянию программы."),
        ("6 Достоинства и недостатки", "Метод позволяет находить ошибки ветвления, пропущенные переходы и ошибки работы циклов, а также измерять покрытие кода. Он требует доступа к реализации; число маршрутов может быть большим, а отсутствующую в коде функциональность или ошибку требований полное покрытие само по себе не выявляет."),
        ("7 Стратегии белого ящика", "Покрытие операторов требует выполнить каждый оператор. Покрытие решений - получить оба исхода каждого решения. Покрытие условий - получить истину и ложь для простых условий. Покрытие решений и условий объединяет эти проверки; комбинаторное покрытие рассматривает все достижимые сочетания условий. Метод базового пути выбирает структурно независимые маршруты."),
        ("8 Тестирование базового пути", "По графу управления вычисляют цикломатическую сложность, выделяют базис независимых путей, подбирают данные и выполняют проверки. Нереализуемые пути отмечают с объяснением ограничений; их нельзя объявлять проверенными на допустимых данных."),
        ("9 Особенности потокового графа", "Вершины соответствуют линейным участкам кода и условиям, ориентированные дуги - переходам управления. У простого предиката два исхода. Составное условие разбивают на простые проверки с учётом короткого замыкания. При подсчёте областей учитывают внешнюю область."),
        ("10 Цикломатическая сложность", "Это количественная характеристика структуры управления и размер независимого базиса графа. Для связного графа с одним входом и выходом V(G) = E - N + 2; при бинарных предикатах также V(G) = P + 1. Для плоского изображения результат равен числу областей с внешней."),
        ("11 Базовое множество", "Базовое множество состоит из независимых путей графа. В выбранном порядке каждый следующий путь содержит ранее не использованную дугу. В данной работе P1, P2 и P3 образуют структурный базис; P1 нельзя реализовать в математической области n ≥ 0."),
        ("12 Шаги метода базового пути", "Пронумеровать участки исходного кода, построить граф, рассчитать сложность, выделить независимые пути и проверить их достижимость. Затем подобрать входные данные и ожидаемые результаты, выполнить тесты, сравнить фактическое поведение и оформить выводы."),
    ]
    p6 = [("h", "Ответы на контрольные вопросы")]
    p7 = [("h", "Ответы на контрольные вопросы продолжение")]
    for index, (heading, answer) in enumerate(questions):
        (p6 if index < 6 else p7).extend([("subh", heading), ("p", answer)])
    p7 += [
        ("subh", "Вывод"),
        ("p", "Для вычисления Lₖ(t) построен граф, тремя способами получена сложность 3 и выделен структурный базис. Фактические значения и маршруты совпали с ожидаемыми. Все дуги вычислительного графа покрыты допустимыми примерами; ограничение пути с нулём итераций явно отражено в результатах."),
        ("subh", "Источник"),
        ("p", "Тестирование ПО.pdf. Методические указания. Лабораторная работа 3: с. 12-29; порядок выполнения: с. 26; вариант 3: с. 27; требования к отчёту и контрольные вопросы: с. 29."),
    ]
    return [p1, p2, p3, p4, p5, p6, p7]


def markdown(pages):
    lines = []
    for page in pages:
        for kind, value in page:
            if kind in ("title", "h", "subh"):
                lines.append({"title": "# ", "h": "## ", "subh": "### "}[kind] + value)
            elif kind == "subtitle":
                lines.append("**" + value + "**")
            elif kind == "code":
                lines.append("```text\n" + value + "\n```")
            elif kind == "table":
                header, rows, _ = value
                lines.append("| " + " | ".join(header) + " |\n| " + " | ".join(["---"] * len(header)) +
                             " |\n" + "\n".join("| " + " | ".join(row) + " |" for row in rows))
            elif kind == "image":
                lines.append("![Потоковый граф вычисления L_k](images/" + value + ")")
            elif kind == "formula":
                lines.append("$$\n" + value + "\n$$")
            else:
                lines.append(value)
            lines.append("")
    (DOCS / "LAB3.md").write_text("\n".join(lines), encoding="utf-8")


def pdf(pages):
    fonts = Path("C:/Windows/Fonts")
    for family, file in [("Times", "times.ttf"), ("TimesBold", "timesbd.ttf"),
                         ("Arial", "arial.ttf"), ("ArialBold", "arialbd.ttf"), ("Mono", "consola.ttf")]:
        pdfmetrics.registerFont(TTFont("Lab" + family, str(fonts / file)))
    pdfmetrics.registerFontFamily("LabTimes", normal="LabTimes", bold="LabTimesBold", italic="LabTimes", boldItalic="LabTimesBold")
    normal = ParagraphStyle("LabBody", fontName="LabTimes", fontSize=11.5, leading=15.3, spaceAfter=7)
    styles = {
        "p": normal,
        "title": ParagraphStyle("LabTitle", parent=normal, fontName="LabArialBold", fontSize=19, leading=23, spaceAfter=12),
        "subtitle": ParagraphStyle("LabSubtitle", parent=normal, fontName="LabArial", fontSize=13, leading=17, spaceAfter=10),
        "h": ParagraphStyle("LabHeading", parent=normal, fontName="LabArialBold", fontSize=14, leading=18, spaceBefore=7, spaceAfter=9, keepWithNext=True),
        "subh": ParagraphStyle("LabSubheading", parent=normal, fontName="LabArialBold", fontSize=11.3, leading=14, spaceBefore=5, spaceAfter=4, keepWithNext=True),
        "code": ParagraphStyle("LabCode", fontName="LabMono", fontSize=9.1, leading=12.6, spaceAfter=10),
        "cell": ParagraphStyle("LabCell", fontName="LabTimes", fontSize=10.5, leading=13, spaceAfter=0),
        "center": ParagraphStyle("LabCenter", fontName="LabTimes", fontSize=10.5, leading=13, alignment=TA_CENTER),
    }
    story = []
    for index, page in enumerate(pages):
        if index:
            story.append(PageBreak())
        for kind, value in page:
            if kind == "table":
                header, rows, widths = value
                table_rows = []
                for row_index, row in enumerate([header] + rows):
                    table_rows.append([Paragraph(html.escape(text), styles["center"] if col == 0 or len(row) == 6 else styles["cell"])
                                       for col, text in enumerate(row)])
                table = Table(table_rows, colWidths=widths, repeatRows=1, hAlign="LEFT")
                table.setStyle(TableStyle([
                    ("GRID", (0, 0), (-1, -1), .5, colors.HexColor("#D9D9D9")),
                    ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#ECEFF1")),
                    ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
                    ("TOPPADDING", (0, 0), (-1, -1), 6), ("BOTTOMPADDING", (0, 0), (-1, -1), 6),
                    ("LEFTPADDING", (0, 0), (-1, -1), 7), ("RIGHTPADDING", (0, 0), (-1, -1), 7),
                ]))
                story.extend([table, Spacer(1, 9)])
            elif kind == "code":
                story.append(Preformatted(value, styles["code"]))
            elif kind == "image":
                from PIL import Image as PILImage
                with PILImage.open(ASSETS / value) as im:
                    width, height = im.size
                story.append(Image(str(ASSETS / value), width=355, height=355 * height / width))
                story.append(Spacer(1, 5))
            elif kind == "formula":
                story.append(equation())
                story.append(Spacer(1, 5))
            else:
                story.append(Paragraph(html.escape(value), styles[kind]))
    def footer(canvas, doc):
        canvas.saveState()
        canvas.setFont("LabArial", 9)
        canvas.drawCentredString(A4[0] / 2, 23, str(doc.page))
        canvas.restoreState()
    doc = SimpleDocTemplate(str(OUTPUT), pagesize=A4, leftMargin=45, rightMargin=45,
                            topMargin=37, bottomMargin=40,
                            title="Лабораторная работа 3 Вариант 3 Многочлен Лагранжа",
                            author="", pageCompression=1)
    doc.build(story, onFirstPage=footer, onLaterPages=footer)


def main():
    ASSETS.mkdir(parents=True, exist_ok=True)
    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    data = json.loads((RESULTS / "lab3-cases.json").read_text(encoding="utf-8"))
    trx = ET.parse(RESULTS / "lab3.trx").getroot()
    ns = {"t": "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"}
    counters = trx.find("t:ResultSummary/t:Counters", ns).attrib
    ids = {item.attrib["id"] for item in trx.findall("t:TestDefinitions/t:UnitTest", ns)
           if "LagrangeBasisTests" in item.find("t:TestMethod", ns).attrib.get("className", "")}
    lab_count = sum(result.attrib.get("testId") in ids for result in trx.findall("t:Results/t:UnitTestResult", ns))
    assert lab_count == 22, lab_count
    graph()
    pages = build_pages(data, counters, lab_count)
    markdown(pages)
    pdf(pages)
    print(OUTPUT)
    print(DOCS / "LAB3.md")


if __name__ == "__main__":
    main()
