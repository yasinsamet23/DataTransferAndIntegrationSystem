import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";

const pageClass =
    "min-h-screen flex items-center justify-center bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900";

const cardClass =
    "bg-white rounded-2xl shadow-2xl p-10 w-full max-w-md";

const labelClass =
    "block mb-2 font-medium text-slate-700";

const inputClass =
    "w-full border rounded-lg px-4 py-3 outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500";

const passwordInputClass =
    `${inputClass} pr-12`;

const loginButtonClass =
    "w-full bg-blue-600 hover:bg-blue-700 text-white py-3 rounded-lg transition disabled:opacity-50";

const errorClass =
    "bg-red-100 border border-red-300 text-red-600 rounded-lg p-3 text-sm";

function Login() {

    const navigate = useNavigate();

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const [showPassword, setShowPassword] = useState(false);

    useEffect(() => {

        const token = localStorage.getItem("token");

        if (token) {

            navigate("/");

        }

    }, [navigate]);

    const handleLogin = async (e) => {

        e.preventDefault();

        setLoading(true);
        setError("");

        try {

            const response = await api.post("/Auth/login", {
                username,
                password
            });

            localStorage.setItem(
                "token",
                response.data.token
            );

            navigate("/");

        }
        catch {

            setError("Invalid username or password.");

        }
        finally {

            setLoading(false);

        }

    };

    return (

        <div className={pageClass}>

            <div className={cardClass}>

                <div className="text-center mb-8">

                    <div className="text-5xl mb-4">
                        🔄
                    </div>

                    <h1 className="text-3xl font-bold text-slate-800">
                        Data Transfer &amp; Integration System
                    </h1>

                    <p className="text-sm text-slate-500 mt-3">
                        Welcome Back
                    </p>

                </div>

                <form
                    onSubmit={handleLogin}
                    className="space-y-5"
                >

                    <div>

                        <label className={labelClass}>
                            Username
                        </label>

                        <input
                            type="text"
                            value={username}
                            onChange={(e) =>
                                setUsername(e.target.value)
                            }
                            placeholder="Enter username"
                            className={inputClass}
                        />

                    </div>

                    <div>

                        <label className={labelClass}>
                            Password
                        </label>

                        <div className="relative">

                            <input
                                type={
                                    showPassword
                                        ? "text"
                                        : "password"
                                }
                                value={password}
                                onChange={(e) =>
                                    setPassword(e.target.value)
                                }
                                placeholder="Enter password"
                                className={passwordInputClass}
                            />

                            <button
                                type="button"
                                onClick={() =>
                                    setShowPassword(!showPassword)
                                }
                                className="absolute right-3 top-3 text-slate-500"
                            >
                                {showPassword ? "🙈" : "👁️"}
                            </button>

                        </div>

                    </div>

                    {error && (

                        <div className={errorClass}>
                            {error}
                        </div>

                    )}

                    <button
                        type="submit"
                        disabled={loading}
                        className={loginButtonClass}
                    >
                        {
                            loading
                                ? "Signing In..."
                                : "Login"
                        }
                    </button>

                </form>

            </div>

        </div>

    );

}

export default Login;