function viewForm() {
    $(this).removeClass()
    $(this).addClass("spinner-grow spinner-grow-sm")
    let formId = this.dataset.form
    $("#pdfFileInput").val()

    let myHeaders = new Headers();
    myHeaders.append("Content-type", "application/pdf");
    let myInit = {
        method: "GET",
        headers: myHeaders
    };

    const url = "/api/Form/GetForm/" + formId

    fetch(url, myInit)
        .then(res => res.blob())
        .then(blob => {
            var file = window.URL.createObjectURL(blob);
            window.open(file, '_blank').focus();
            $(this).removeClass()
            $(this).addClass("bi bi-eye-fill view-form")
        });

}