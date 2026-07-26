import { Link, useNavigate } from "react-router-dom";


function Sidebar() {
  const navigate = useNavigate();
  const handleLogout = () => {

    localStorage.removeItem("token");

    navigate("/login");
  };
  return (

    <div className="w-64 bg-slate-900 text-white min-h-screen">

      <h2 className="text-xl font-bold p-6">

        Dashboard

      </h2>

      <nav className="flex flex-col">

        <Link
          className="px-6 py-3 hover:bg-slate-700"
          to="/"
        >
          Dashboard
        </Link>

        <Link
          className="px-6 py-3 hover:bg-slate-700"
          to="/users"
        >
          Users
        </Link>

        <Link
          className="px-6 py-3 hover:bg-slate-700"
          to="/transfer-logs"
        >
          Transfer Logs
        </Link>

        <Link
          className="px-6 py-3 hover:bg-slate-700"
          to="/error-logs"
        >
          Error Logs
        </Link>

      </nav>
      <button
        onClick={handleLogout}
        className="w-full text-left px-6 py-3 hover:bg-slate-700 transition cursor-pointer"
      >
        Logout
      </button>

    </div>
  );
}

export default Sidebar;