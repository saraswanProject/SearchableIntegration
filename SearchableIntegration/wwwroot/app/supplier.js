const SupplierList = {
    template: '#supplier-template',
    data() {
        return {
            suppliers: [],

            newSupplier: {
                supplierName: '',
                contactNumber: '',
                email: '',
                address: '',
                city: '',
                country: ''
            },
            editId: null,
            currentPage: 1,
            pageSize: 5,
            loading: false
        };
    },
    computed: {
        pagedProducts() {
            const start = (this.currentPage - 1) * this.pageSize;
            return this.suppliers.slice(start, start + this.pageSize);
        },
        totalPages() {
            return Math.ceil(this.suppliers.length / this.pageSize);
        }
    },
    methods: {
        async loadSuppliers() {
            this.loading = true;
            try {
                const res = await fetch('/api/supplierapi/getsuppliers');
                this.suppliers = await res.json();
              
            } catch (error) {
                console.error('Error loading supplier:', error);
                alert('Failed to load supplier');
            } finally {
                this.loading = false;
            }
        },
    
        async addSupplier() {
            const p = this.newSupplier;
            if (!p.supplierName || !p.contactNumber || !p.email || !p.address || !p.city || !p.country) {
                return alert("Please fill all required fields.");
            }

            const formData = new FormData();
            formData.append("SupplierName", p.supplierName);
            formData.append("ContactNumber", p.contactNumber);
            formData.append("Email", p.email);
            formData.append("Address", p.address);
            formData.append("City", p.city);
            formData.append("Country", p.country);

            try {
                const res = await fetch('/api/supplierapi/create', {
                    method: 'POST',
                    body: formData
                });

                if (res.ok) {
                    const response = await res.json();
                    alert(response.message);
                    if (response.code == "200") {
                        this.resetForm();
                        await this.loadSuppliers();
                    }
                   
                } else {
                    const error = await res.json();
                    throw new Error(error.message || 'Failed to create product');
                }
            } catch (error) {
                console.error('Error adding supplier:', error);
                alert(error.message);
            }
        },
        resetForm() {
            this.newSupplier = {
                supplierName: '',
                contactNumber: '',
                email: '',
                address: '',
                city: '',
                country: ''
            };
        
            this.editId = null;
        },
     
        async deleteSupplier(supplierID) {
            if (!confirm('Are you sure you want to delete this product?')) return;

            try {
                const res = await fetch(`/api/supplierapi/delete/${supplierID}`, {
                    method: 'DELETE'
                });

                if (res.ok) {
                    const response = await res.json();
                    alert(response.message);
                    if (response.code == "200") {
                        this.resetForm();
                        await this.loadSuppliers();
                    }
               
                } else {
                    const error = await res.json();
                    throw new Error(error.message || 'Failed to delete supplier');
                }
            } catch (error) {
                console.error('Error deleting supplier:', error);
                alert(error.message);
            }
        },
        editSupplier(supplier) {
            debugger
            this.editId = supplier.supplierID;
            this.newSupplier = {
                supplierId: supplier.supplierID, 
                supplierName: supplier.supplierName,
                contactNumber: supplier.contactNumber,
                email: supplier.email,
                address: supplier.address,
                city: supplier.city,
                country: supplier.country
          
            };
        },
      
        async submitForm() {
            debugger
            if (this.editId) {
                console.log("Calling updateSupplier");
                await this.updateSupplier();
            } else {
                console.log("Calling addSupplier");
                await this.addSupplier();
            }
        },

        async updateSupplier() {
            console.log("Update called");
            try {
                const res = await fetch('/api/supplierapi/update', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(this.newSupplier)
                });

                if (res.ok) {
                    this.resetForm();
                    await this.loadSuppliers();
                } else {
                    const error = await res.json();
                    throw new Error(error.message || 'Failed to update supplier');
                }
            } catch (error) {
                console.error('Error updating supplier:', error);
                alert(error.message);
            }
        },
        goToPage(page) {
            if (page >= 1 && page <= this.totalPages) {
                this.currentPage = page;
            }
        }
      
    },
    async mounted() {
        await this.loadSuppliers();
     
    }
};

const app = Vue.createApp({});
app.component('supplier-list', SupplierList);
app.mount('#supplier-app');