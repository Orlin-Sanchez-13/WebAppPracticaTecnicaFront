import { TablePagination } from './tablePagination.js';

export class ParticipantesTable {
    constructor(participantesData) {
        this.rawData = participantesData;
        this.processedData = this.transformData(participantesData);
    }

    transformData(data) {
        return data.map(p => ({
            compania: p.nombreCompania,
            cedula: p.cedula,
            contacto: p.nombreContacto,
            titulo: p.titulo,
            correo: p.correo,
            telefono: p.telefono
        }));
    }

    renderRow(registro) {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${registro.compania}</td>
            <td>${registro.cedula}</td>
            <td>${registro.contacto}</td>
            <td>${registro.titulo}</td>
            <td>${registro.correo}</td>
            <td>${registro.telefono}</td>
        `;
        return tr;
    }

    init() {
        const paginationHandler = new TablePagination(this.processedData, {
            pageSize: 10,
            tableBodyId: 'registrosBody',
            paginationId: 'pagination',
            renderRowCallback: (item) => this.renderRow(item),
            emptyMessage: 'No hay participantes registrados'
        });

        paginationHandler.init();
        return paginationHandler;
    }
}