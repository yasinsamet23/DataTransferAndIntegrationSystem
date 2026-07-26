import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";

function Login() {
    const navigate = useNavigate();

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleLogin = async (e) => {
        e.preventDefault();

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
    };

    return (
        <div style={{ padding: "40px" }}>
            <h2>Login</h2>

            <form onSubmit={handleLogin}>

                <div>
                    <label>Username</label>
                    <br />
                    <input
                        type="text"
                        value={username}
                        onChange={(e) =>
                            setUsername(e.target.value)}
                    />
                </div>

                <br />

                <div>
                    <label>Password</label>
                    <br />
                    <input
                        type="password"
                        value={password}
                        onChange={(e) =>
                            setPassword(e.target.value)}
                    />
                </div>

                <br />

                <button type="submit">
                    Login
                </button>

            </form>

            <br />

            {error && (
                <p style={{ color: "red" }}>
                    {error}
                </p>
            )}

        </div>
    );
}

export default Login;