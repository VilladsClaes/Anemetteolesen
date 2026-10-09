// Spørg før sletning: formularer med data-bekraeft="..." skal bekræftes
document.addEventListener("submit", function (e) {
    var spoergsmaal = e.target.getAttribute("data-bekraeft");
    if (spoergsmaal && !window.confirm(spoergsmaal)) {
        e.preventDefault();
    }
});
