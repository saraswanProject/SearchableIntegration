const ProductList = {
    template: '#product-template',
    data() {
        return {
            products: [],
            suppliers: [],
            newProduct: {
                name: '',
                category: '',
                price: '',
                stockQuantity: '',
                supplierID: '',
                manufacturedDate: '',
                expiryDate: '',
                description: ''
            },
            imageFile: null,
            previewUrl: null,
            editId: null,
            currentPage: 1,
            pageSize: 5,
            loading: false
        };
    },
    computed: {
        pagedProducts() {
            const start = (this.currentPage - 1) * this.pageSize;
            return this.products.slice(start, start + this.pageSize);
        },
        totalPages() {
            return Math.ceil(this.products.length / this.pageSize);
        }
    },
    methods: {
        async loadProducts() {
            this.loading = true;
            try {
                const res = await fetch('/api/productapi/getproducts');
                this.products = await res.json();
                console.log(this.products)
            } catch (error) {
                console.error('Error loading products:', error);
                alert('Failed to load products');
            } finally {
                this.loading = false;
            }
        },
        async loadSuppliers() {
            try {
                const res = await fetch('/api/productapi/getsuppliers');
                this.suppliers = await res.json();
            } catch (error) {
                console.error('Error loading suppliers:', error);
                alert('Failed to load suppliers');
            }
        },
        async addProduct() {
            const p = this.newProduct;
            if (!p.name || !p.category || !p.price || !p.stockQuantity || !p.supplierID || !p.manufacturedDate) {
                return alert("Please fill all required fields.");
            }

            const formData = new FormData();
            formData.append("Name", p.name);
            formData.append("Category", p.category);
            formData.append("Price", p.price);
            formData.append("StockQuantity", p.stockQuantity);
            formData.append("SupplierID", p.supplierID);
            formData.append("ManufacturedDate", p.manufacturedDate);
            if (p.expiryDate) formData.append("ExpiryDate", p.expiryDate);
            if (p.description) formData.append("Description", p.description);
            if (this.imageFile) formData.append("image", this.imageFile);

            try {
                const res = await fetch('/api/productapi/create', {
                    method: 'POST',
                    body: formData
                });

                if (res.ok) {
                    alert(error.message);
                    this.resetForm();
                    await this.loadProducts();
                } else {
                    const error = await res.json();
                    throw new Error(error.message || 'Failed to create product');
                }
            } catch (error) {
                console.error('Error adding product:', error);
                alert(error.message);
            }
        },
        resetForm() {
            this.newProduct = {
                name: '',
                category: '',
                price: '',
                stockQuantity: '',
                supplierID: '',
                manufacturedDate: '',
                expiryDate: '',
                description: ''
            };
            this.imageFile = null;
            this.previewUrl = null;
            this.editId = null;
        },
        handleFileUpload(event) {
            debugger
            const file = event.target.files[0];
            if (!file) return;

            if (!file.type.startsWith('image/')) {
                alert('Please select an image file');
                return;
            }

            if (file.size > 4 * 1024 * 1024) {
                alert("Image must be less than 4MB.");
                return;
            }

            this.imageFile = file;
            this.previewUrl = URL.createObjectURL(file);
        },
        async deleteProduct(id) {
            if (!confirm('Are you sure you want to delete this product?')) return;

            try {
                const res = await fetch(`/api/productapi/delete/${id}`, {
                    method: 'DELETE'
                });

                if (res.ok) {
                    await this.loadProducts();
                } else {
                    const error = await res.json();
                    throw new Error(error.message || 'Failed to delete product');
                }
            } catch (error) {
                console.error('Error deleting product:', error);
                alert(error.message);
            }
        },
        editProduct(product) {
            debugger
            this.editId = product.id;

            this.newProduct = {
                id: product.id, 
                name: product.name,
                category: product.category,
                price: product.price,
                stockQuantity: product.stockQuantity,
                supplierID: product.supplierID,
                manufacturedDate: product.manufacturedDate.split('T')[0], // Format date for input[type=date]
                expiryDate: product.expiryDate ? product.expiryDate.split('T')[0] : '',
                description: product.description || ''
            };

            if (product.imagePath) {
                this.previewUrl = product.imagePath;
            }
        },
      
        async submitForm() {
            debugger
            if (this.editId) {
                console.log("Calling updateProduct");
                await this.updateProduct();
            } else {
                console.log("Calling addProduct");
                await this.addProduct();
            }
        },

        async updateProduct() {
            console.log("Update called");
            try {
                const res = await fetch('/api/productapi/update', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(this.newProduct)
                });

                if (res.ok) {
                    this.resetForm();
                    await this.loadProducts();
                } else {
                    const error = await res.json();
                    throw new Error(error.message || 'Failed to update product');
                }
            } catch (error) {
                console.error('Error updating product:', error);
                alert(error.message);
            }
        },
        goToPage(page) {
            if (page >= 1 && page <= this.totalPages) {
                this.currentPage = page;
            }
        },
        formatDate(dateString) {
            if (!dateString) return 'N/A';
            const date = new Date(dateString);
            return date.toLocaleDateString();
        }
    },
    async mounted() {
        await this.loadSuppliers();
        await this.loadProducts();
    }
};

const app = Vue.createApp({});
app.component('product-list', ProductList);
app.mount('#product-app');