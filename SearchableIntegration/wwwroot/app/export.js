
import { createApp } from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js';


   const Export = {
    template: '#export-template',
        data() {
            return {
                selectedEntity: 'products',
                printData: []
            };
        },
        filters: {
            capitalize(value) {
                if (!value) return '';
                value = value.toString();
                return value.charAt(0).toUpperCase() + value.slice(1);
            }
        },
        methods: {
            async exportData(format) {
                try {
                    const url = `/api/export/export/${this.selectedEntity}/${format}`;

                    // Create a temporary anchor element to trigger the download
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = `${this.selectedEntity}.${format}`;
                    document.body.appendChild(a);
                    a.click();
                    document.body.removeChild(a);
                } catch (error) {
                    console.error('Export error:', error);
                    this.$toast.error(`Failed to export ${this.selectedEntity} data`);
                }
            },
            async printViewData() {
                try {
                    const response = await this.$http.get(`/api/export/print/${this.selectedEntity}`);
                    if (response.data.success) {
                        this.printData = response.data.data;
                        $('#printPreviewModal').modal('show');
                    } else {
                        this.$toast.error(response.data.message);
                    }
                } catch (error) {
                    console.error('Print error:', error);
                    this.$toast.error(`Failed to load ${this.selectedEntity} data for printing`);
                }
            },
            printNow() {
                $('#printPreviewModal').modal('hide');
                setTimeout(() => {
                    const printContent = document.querySelector('.modal-content').cloneNode(true);
                    const printWindow = window.open('', '_blank');

                    printWindow.document.write(`
          <html>
            <head>
              <title>${this.selectedEntity} Report</title>
              <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css">
              <style>
                @media print {
                  body { padding: 20px; }
                  .print-header { margin-bottom: 20px; text-align: center; }
                  .table { width: 100%; margin-bottom: 1rem; color: #212529; }
                  .table th, .table td { padding: 0.75rem; vertical-align: top; border-top: 1px solid #dee2e6; }
                  .table thead th { vertical-align: bottom; border-bottom: 2px solid #dee2e6; }
                  .table-bordered { border: 1px solid #dee2e6; }
                  .table-bordered th, .table-bordered td { border: 1px solid #dee2e6; }
                  .table-striped tbody tr:nth-of-type(odd) { background-color: rgba(0, 0, 0, 0.05); }
                }
              </style>
            </head>
            <body>
              ${printContent.innerHTML}
              <script>
                window.onload = function() {
                  window.print();
                  window.close();
                };
              <\/script>
            </body>
          </html>
        `);

                    printWindow.document.close();
                }, 500);
            },
            formatValue(value) {
                if (value === null || value === undefined) return 'N/A';
                if (typeof value === 'object' && value instanceof Date) {
                    return value.toLocaleDateString();
                }
                return value.toString();
            },
            capitalize(str) {
                if (!str) return '';
                return str.charAt(0).toUpperCase() + str.slice(1);
            }
        }
    };



const app = createApp({});
app.component('export', Export);
app.mount('#export-app');
