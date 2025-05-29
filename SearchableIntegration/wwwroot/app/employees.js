import { createApp } from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js';
import Employee from './model/employee-model.js';

const EmployeesList = {
   
    template: '#employee-template',
    data() {
        return { employees: [] };
    },
    async mounted() {
        debugger
        const res = await fetch('/api/DummyApiIntegration/getemployees');
        const rawData = await res.json();
        debugger
        this.employees = rawData.data.map(p => new Employee(p.id, p.employee_name, p.employee_salary, p.employee_age, p.profile_image));
    }
};

const app = createApp({});
app.component('employee-list', EmployeesList);
app.mount('#employee-app');