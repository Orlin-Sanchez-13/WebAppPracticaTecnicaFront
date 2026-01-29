export class TablePagination {
    constructor(data, options = {}) {
        this.data = data;
        this.pageSize = options.pageSize || 10;
        this.tableBodyId = options.tableBodyId;
        this.paginationId = options.paginationId;
        this.renderRowCallback = options.renderRowCallback;
        this.emptyMessage = options.emptyMessage || 'No hay registros disponibles';
        
        this.tableBody = document.getElementById(this.tableBodyId);
        this.pagination = document.getElementById(this.paginationId);
        
        if (!this.tableBody || !this.pagination) {
            console.error('Elementos de tabla o paginación no encontrados');
            return;
        }
        
        this.currentPage = 1;
    }

    renderPage(page) {
        this.tableBody.innerHTML = '';
        const start = (page - 1) * this.pageSize;
        const pageItems = this.data.slice(start, start + this.pageSize);
        
        if (pageItems.length === 0 && this.data.length === 0) {
            const tr = document.createElement('tr');
            const colspan = this.tableBody.closest('table').querySelectorAll('thead th').length;
            tr.innerHTML = `<td colspan="${colspan}" class="text-center">${this.emptyMessage}</td>`;
            this.tableBody.appendChild(tr);
            this.pagination.innerHTML = '';
            return;
        }

        for (const item of pageItems) {
            const tr = this.renderRowCallback(item);
            this.tableBody.appendChild(tr);
        }
        
        this.currentPage = page;
        this.renderPagination(page);
    }

    renderPagination(activePage) {
        this.pagination.innerHTML = '';
        const totalPages = Math.ceil(this.data.length / this.pageSize);

        if (totalPages <= 1) {
            return;
        }

        const createPageItem = (page, label, disabled = false, active = false) => {
            const li = document.createElement('li');
            li.className = 'page-item' + (disabled ? ' disabled' : '') + (active ? ' active' : '');
            const a = document.createElement('a');
            a.className = 'page-link';
            a.href = '#';
            a.textContent = label;
            a.addEventListener('click', (e) => {
                e.preventDefault();
                if (!disabled) this.renderPage(page);
            });
            li.appendChild(a);
            return li;
        };

        this.pagination.appendChild(createPageItem(Math.max(1, activePage - 1), 'Anterior', activePage === 1));
        
        for (let p = 1; p <= totalPages; p++) {
            this.pagination.appendChild(createPageItem(p, p, false, p === activePage));
        }
        
        this.pagination.appendChild(createPageItem(Math.min(totalPages, activePage + 1), 'Siguiente', activePage === totalPages));
    }

    init() {
        this.renderPage(1);
    }

    updateData(newData) {
        this.data = newData;
        this.renderPage(1);
    }
}