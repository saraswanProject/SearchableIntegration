import { createApp } from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js';
import * as XLSX from 'https://cdn.sheetjs.com/xlsx-0.20.0/package/xlsx.mjs';

const ExportImport = {
    template: '#exportimport-template',
    data() {
        return {
            skippedRows: [],     // messages for display
            skippedData: [],     // structured for table + CSV
            imported: [],
            file: null
        };
    },
    methods: {
        handleFileUpload(e) {
            this.file = e.target.files[0];
        },
        async addExportImport() {
            if (!this.file) return alert('No file selected.');

            const ext = this.file.name.split('.').pop().toLowerCase();
            if (!['csv', 'xlsx'].includes(ext)) return alert('Only CSV or Excel files allowed.');

            const rows = await this.readFile(this.file, ext);

            const valid = [], skipped = [];
            rows.forEach((r, i) => {
                if (!r.ProductName || !r.Category || !r.Price || !r.StockQuantity || !r.SupplierID || !r.ManufacturedDate) {
                    skipped.push({ row: i + 2, reason: "Missing required fields" });
                } else {
                    valid.push(r);
                }
            });

            this.skippedData = skipped;
            this.skippedRows = skipped.map(r => `Row ${r.row}: ${r.reason}`);
            this.imported = valid;

            const res = await fetch('/api/exportimport/upload', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(valid)
            });

            const result = await res.json();
            if (result.skipped?.length) {
                this.skippedData = result.skipped;
                this.skippedRows = result.skipped.map(r => `Row ${r.row}: ${r.reason}`);
            }

            console.log('Upload result:', result);
        },
        async readFile(file, ext) {
            return new Promise((resolve) => {
                const reader = new FileReader();
                reader.onload = (e) => {
                    let rows = [];
                    if (ext === 'csv') {
                        const text = e.target.result;
                        const lines = text.trim().split('\n');
                        const headers = lines[0].split(',');
                        for (let i = 1; i < lines.length; i++) {
                            const values = lines[i].split(',');
                            const obj = {};
                            headers.forEach((h, idx) => obj[h.trim()] = values[idx]?.trim());
                            rows.push(obj);
                        }
                    } else {
                        const wb = XLSX.read(e.target.result, { type: 'binary' });
                        const sheet = wb.Sheets[wb.SheetNames[0]];
                        rows = XLSX.utils.sheet_to_json(sheet);
                    }
                    resolve(rows);
                };
                if (ext === 'csv') reader.readAsText(file);
                else reader.readAsBinaryString(file);
            });
        },
        downloadSkipped() {
            if (!this.skippedData.length) return;

            const headers = ['Row', 'Reason'];
            const rows = this.skippedData.map(r => [r.row, `"${r.reason.replace(/"/g, '""')}"`]);

            let csv = headers.join(',') + '\n' + rows.map(r => r.join(',')).join('\n');

            const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
            const link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.download = 'skipped_rows.csv';
            link.click();
        }
    }
};

const app = createApp({});
app.component('export-import', ExportImport);
app.mount('#exportimport-app');
