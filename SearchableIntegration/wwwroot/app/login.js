import { createApp } from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js';

const login = {
    template: '#login-template',
    data() {
        return {
            credentials: {
                email: '',
                password: ''
            },
            error: '',
            showSignup: false,
            signupData: {
                name: '',
                email: '',
                password: '',
                role: ''
            },
            signupError: ''
        };
    },
    methods: {
        resetForm() {
            this.signupData = {
                name: '',
                email: '',
                price: '',
                password: '',
                role: ''
            };
          
        },

        async submitLogin() {
            console.log("Login initiated");
            const payload = {
                email: this.credentials.email,
                password: this.credentials.password
            };

            try {
                console.log("Login Payload:", payload);

                const res = await fetch('/api/accountapi/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: JSON.stringify(payload)
                });

                if (!res.ok) {
                    const errorText = await res.text();
                    console.error("Login API Error Response:", errorText);
                    throw new Error(errorText || 'Login failed');
                }

                const response = await res.json();
                console.log("Login successful:", response);
                location.href = '/home/index'; // Redirect after login

            } catch (error) {
                console.error('Error during login:', error);
                this.error = error.message || 'Login failed';
                alert(this.error);
            }
        },
        toggleSignup() {
          this.resetForm();
            this.showSignup = !this.showSignup;
            this.signupError = '';
        },
        async submitSignup() {
            try {
                console.log("Signup initiated");
                if (!this.signupData.role) {
                    throw new Error('Please select a user role');
                }

                const payload = {
                    name: this.signupData.name,
                    email: this.signupData.email,
                    password: this.signupData.password,
                    role: this.signupData.role
                };

                const res = await fetch('/api/accountapi/signup', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json'
                    },
                    body: JSON.stringify(payload)
                });

                if (!res.ok) {
                    const errorText = await res.text();
                    console.error("Signup API Error Response:", errorText);
                    throw new Error(errorText || 'Signup failed');
                }

                const response = await res.json();
                console.log("Signup successful:", response);
                alert('Signup successful! Please login with your new credentials.');
                this.showSignup = false;
                this.signupData = { name: '', email: '', password: '' };

            } catch (error) {
                console.error('Error during signup:', error);
                this.signupError = error.message || 'Signup failed';
                alert(this.signupError);
            }
        }
    }
};

const app = createApp({});
app.component('login', login);
app.mount('#login-app');