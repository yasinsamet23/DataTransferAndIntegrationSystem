import { BrowserRouter, Routes, Route } from "react-router-dom";
import Login from "./pages/Login";
import MainLayout from "./layouts/MainLayout";
import Dashboard from "./pages/Dashboard";
import Users from "./pages/Users";
import TransferLogs from "./pages/TransferLogs";
import ErrorLogs from "./pages/ErrorLogs";
import ProtectedRoute from "./components/ProtectedRoute";


function App() {
  return (
    <BrowserRouter>
      <Routes>

        <Route
          path="/login"
          element={<Login />}
        />

        <Route element={<ProtectedRoute />}>

          <Route path="/" element={<MainLayout />}>

            <Route
              index
              element={<Dashboard />}
            />

            <Route
              path="users"
              element={<Users />}
            />

            <Route
              path="transfer-logs"
              element={<TransferLogs />}
            />

            <Route
              path="error-logs"
              element={<ErrorLogs />}
            />

          </Route>

        </Route>

      </Routes>
    </BrowserRouter>
  );
}

export default App;