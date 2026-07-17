import { Link } from "react-router-dom";

function Sidebar() {
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

    </div>
  );
}

export default Sidebar;