import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5207/api"
});

// Her istekten önce çalışır
api.interceptors.request.use((config) => {

    const token = localStorage.getItem("token");

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
});

export default api;