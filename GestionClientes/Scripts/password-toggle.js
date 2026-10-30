// muestra u oculta la contraseña de cualquier input
// se usa con un boton que tenga data-toggle-password="idDelInput"
document.addEventListener("click", function (evento) {
    var boton = evento.target.closest("[data-toggle-password]");

    if (!boton) {
        return;
    }

    var input = document.getElementById(boton.getAttribute("data-toggle-password"));

    if (!input) {
        return;
    }

    var mostrar = input.type === "password";

    input.type = mostrar ? "text" : "password";
    boton.setAttribute("aria-label", mostrar ? "Ocultar contraseña" : "Mostrar contraseña");
    boton.innerHTML = mostrar
        ? '<i class="bi bi-eye-slash"></i>'
        : '<i class="bi bi-eye"></i>';
});
