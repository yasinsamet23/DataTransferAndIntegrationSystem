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

// Her cevap geldikten sonra çalışır
api.interceptors.response.use(

    (response) => {

        return response;

    },

    (error) => {

        if (error.response?.status === 401) {

            localStorage.removeItem("token");

            window.location.href = "/login";

        }

        return Promise.reject(error);

    }

);

export default api;