import { Outlet } from "react-router-dom";

import Navbar from "../components/Navbar";
import Sidebar from "../components/Sidebar";

function MainLayout() {
  return (
    <div className="flex min-h-screen">

      <Sidebar />

      <div className="flex-1">

        <Navbar />

        <main className="p-6 bg-slate-100 min-h-screen">

          <Outlet />

        </main>

      </div>

    </div>
  );
}

export default MainLayout;