// parse date to a format that JS could parse
function parseDate(raw) {
    let clean = raw
        .replace("p. m.", "PM")
        .replace("a. m.", "AM")
        .trim();

    const [date, time, meridian] = clean.split(" ");
    const [day, month, year] = date.split("/");

    const formatted = `${year}-${month}-${day}`;
    return new Date(formatted);
}

document.addEventListener("DOMContentLoaded", () => {
    const dateElements = document.querySelectorAll("[id^='publish-date-']");

    dateElements.forEach((el) => {
        const rawDate = parseDate(el.dataset.date);
        if (!rawDate) return;

        const formatted = new Date(rawDate).toLocaleDateString("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric",
        });

        el.textContent = `Publish Date: ${formatted}`;
    });
});


function recortar(texto, max = 200)
{
    if (string.IsNullOrWhiteSpace(texto)) return "";
    return texto.Length <= max ? texto : texto.Substring(0, max) + "...";
}

