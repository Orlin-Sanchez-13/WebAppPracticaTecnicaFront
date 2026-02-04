document.addEventListener('DOMContentLoaded', function () {
    var config = window.registrarData || {};
    var form = document.getElementById('registroForm');
    var submitBtn = document.getElementById('btnRegistrar');
    var inputs = form.querySelectorAll('input.form-control');
    var termsCheck = document.getElementById('aceptaTerminos');

    function updateSubmitState() {
        var allFilled = Array.prototype.every.call(inputs, function (input) {
            return input.value.trim().length > 0;
        });
        var termsOk = termsCheck && termsCheck.checked;
        submitBtn.disabled = !(allFilled && termsOk);
    }

    inputs.forEach(function (input) {
        input.addEventListener('input', updateSubmitState);
        input.addEventListener('change', updateSubmitState);
    });

    if (termsCheck) {
        termsCheck.addEventListener('change', updateSubmitState);
    }

    updateSubmitState();

    var resultStatus = config.resultStatus;
    var resultData = config.resultData;
    var resultErrors = config.resultErrors;
    var consultaUrl = config.consultaUrl;

    if (!resultStatus) {
        return;
    }

    function applyResultDataToForm(data) {
        if (!data) {
            return;
        }

        var fieldMap = {
            NombreCompania: 'Participante.NombreCompania',
            Cedula: 'Participante.Cedula',
            NombreContacto: 'Participante.NombreContacto',
            Titulo: 'Participante.Titulo',
            Correo: 'Participante.Correo',
            Telefono: 'Participante.Telefono'
        };

        Object.keys(fieldMap).forEach(function (key) {
            var input = form.querySelector('[name="' + fieldMap[key] + '"]');
            if (input && data[key] !== undefined && data[key] !== null) {
                input.value = data[key];
            }
        });

        updateSubmitState();
    }

    var dataContainer = document.getElementById('resultData');
    if (resultData) {
        var fieldRows = [
            { key: 'NombreCompania', label: 'Compañía', value: resultData.NombreCompania },
            { key: 'Cedula', label: 'Cédula', value: resultData.Cedula },
            { key: 'NombreContacto', label: 'Contacto', value: resultData.NombreContacto },
            { key: 'Titulo', label: 'Título', value: resultData.Titulo },
            { key: 'Correo', label: 'Correo', value: resultData.Correo },
            { key: 'Telefono', label: 'Teléfono', value: resultData.Telefono }
        ];

        fieldRows.forEach(function (field) {
            var errorMessages = resultErrors && resultErrors[field.key] ? resultErrors[field.key] : null;

            var wrapper = document.createElement('div');
            wrapper.className = 'mb-2';

            var label = document.createElement('div');
            label.className = errorMessages ? 'fw-semibold text-danger' : 'fw-semibold';
            label.textContent = field.label;

            var value = document.createElement('div');
            value.className = errorMessages ? 'text-danger' : '';
            value.textContent = field.value ?? '';

            wrapper.appendChild(label);
            wrapper.appendChild(value);

            if (errorMessages) {
                var msg = document.createElement('div');
                msg.className = 'text-danger small';
                msg.textContent = errorMessages.join(' ');
                wrapper.appendChild(msg);
            }

            dataContainer.appendChild(wrapper);
        });
    } else {
        dataContainer.classList.add('d-none');
    }

    if (resultStatus === "error") {
        applyResultDataToForm(resultData);
    }

    var modalElement = document.getElementById('resultModal');
    var modal = new bootstrap.Modal(modalElement);
    modal.show();

    if (resultStatus === "success") {
        modalElement.addEventListener('hidden.bs.modal', function () {
            window.location.href = consultaUrl;
        });
    }
});