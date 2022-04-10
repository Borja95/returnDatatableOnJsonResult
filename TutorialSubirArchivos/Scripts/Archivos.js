
$(document).ready(function () {
    //convertirBase64();
    if ($('#containerTablaDescargar').is(':visible')) {    // you get the idea...
        obtenerArchivos();
    }
});

function convertirBase64() {
    $.ajax({
        type: "GET",
        url: "/Home/ConvertToBase64",
        success: function (response) {

            console.log(response);


        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
            alert("Ocurrió un error al verificar los CFDI(s): " + jqXHR);
        }
    });
}


function subirArchivos() {
    var totalFiles = document.getElementById('inputSubirArchivos').files.length;
    var formData = new FormData();

    var esValido = true;
    if (totalFiles == 0) {
        esValido = false;
    }

    if (esValido == true) {

        for (var i = 0; i < totalFiles; i++) {
            formData.append("archivos", document.getElementById("inputSubirArchivos").files[i]);
        }

        $.ajax({
            type: "POST",
            url: "/Home/InsertarArchivos",
            data: formData,
            contentType: false,
            processData: false,
            cache: false,
            beforeSend: function () {
                $("#loaderArchivos").html("<p>SUBIENDO ARCHIVOS...</p>");
            },
            complete: function () {

            },
            success: function (response) {
                var respuesta = response;
                console.log(respuesta);

                $("#loaderArchivos").html(`<p>${respuesta.Mensaje_Respuesta}</p>`);


            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
                alert("Ocurrió un error al verificar los CFDI(s): " + jqXHR);
            }
        });

    }

}

function obtenerArchivos() {

    $.ajax({
        type: "GET",
        url: "/Home/ConsultarArchivos",
        beforeSend: function () {
            $("#bodyTablaDescargar").html("<tr><td colspan='5'>CARGANDO ARCHIVOS...</td></tr>");
        },
        complete: function () {

        },
        success: function (response) {
            var respuesta = response;
            console.log(respuesta);

            
            if (respuesta.Codigo == 1) {
                alert("Éxito");
            } else {
                alert("No ha retornado de forma correcta");
            }
            


        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(jqXHR);
            console.log(textStatus);
            console.log(errorThrown);
            alert("Ocurrió un error al verificar los CFDI(s): " + jqXHR);
        }
    });

}