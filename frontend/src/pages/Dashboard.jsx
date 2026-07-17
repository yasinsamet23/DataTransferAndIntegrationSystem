import { useEffect, useState } from "react";
import api from "../services/api";
import SummaryCard from "../components/SummaryCard";

function Dashboard() {
  const [users, setUsers] = useState([]);
  const [transferLogs, setTransferLogs] = useState([]);

  useEffect(() => {

      loadUsers();

  }, []);


  const loadUsers = async () => {

      try {

          const response = await api.get("/users");

          setUsers(response.data);

      }

      catch (error) {

          console.error(error);

      }

  };
  return (
    <div>
      <h1 className="text-3xl font-bold mb-2">
        Dashboard
      </h1>

      <p className="text-gray-500 mb-8">
        Monitor transfers, records and system health.
      </p>

      {/* Summary Cards */}
      <div className="grid grid-cols-2 gap-6">
        <SummaryCard
          title="Total Users"
          value={users.length}
          color="bg-blue-100"
        />

        <SummaryCard
          title="Total Users"
          value={users.length}
          color="bg-blue-100"
        />

        <SummaryCard
          title="Failed Records"
          value="0"
          color="bg-red-100"
        />

        <SummaryCard
          title="Error Rate"
          value="%0"
          color="bg-yellow-100"
        />
      </div>

      {/* Start Transfer */}
      <div className="mt-8 bg-white rounded-xl shadow-md p-6 flex justify-between items-center">
        <div>
          <h2 className="text-xl font-bold">
            Ready to Transfer
          </h2>

          <p className="text-gray-500">
            Synchronize data from external API.
          </p>
        </div>

        <button className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700">
          Start Transfer
        </button>
      </div>

      {/* Recent Transfer Logs */}
      <div className="mt-8 bg-white rounded-xl shadow-md border">
        <div className="flex justify-between items-center p-6 border-b">
          <div>
            <h2 className="text-xl font-bold">
              Recent Transfer Logs
            </h2>

            <p className="text-gray-500">
              Latest data sync operations across systems.
            </p>
          </div>

          <button className="text-blue-600 font-semibold hover:underline">
            View All
          </button>
        </div>

        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="text-left p-4">Transfer Date</th>
              <th className="text-left p-4">Total Records</th>
              <th className="text-left p-4">Success Count</th>
              <th className="text-left p-4">Status</th>
            </tr>
          </thead>

          <tbody>
            <tr className="border-t">
              <td className="p-4">16 Jul 2026 - 09:42</td>
              <td className="p-4">30</td>
              <td className="p-4">26</td>
              <td className="p-4">
                <span className="bg-green-100 text-green-700 px-3 py-1 rounded-full">
                  Completed
                </span>
              </td>
            </tr>

            <tr className="border-t">
              <td className="p-4">16 Jul 2026 - 08:15</td>
              <td className="p-4">30</td>
              <td className="p-4">24</td>
              <td className="p-4">
                <span className="bg-yellow-100 text-yellow-700 px-3 py-1 rounded-full">
                  Completed With Errors
                </span>
              </td>
            </tr>

            <tr className="border-t">
              <td className="p-4">15 Jul 2026 - 17:33</td>
              <td className="p-4">30</td>
              <td className="p-4">0</td>
              <td className="p-4">
                <span className="bg-red-100 text-red-700 px-3 py-1 rounded-full">
                  Failed
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  );
}


export default Dashboard;