import { useEffect, useState } from "react";
import api from "../services/api";
import StatusBadge from "../components/StatusBadge";

function TransferLogs() {

  const [transferLogs, setTransferLogs] = useState([]);

  useEffect(() => {

    loadTransferLogs();

  }, []);

  const loadTransferLogs = async () => {

    try {

      const response = await api.get("/logs");

      setTransferLogs(response.data);

    }

    catch (error) {

      console.error(error);

    }

  };

  return (

    <div>

      <h1 className="text-3xl font-bold mb-2">
        Transfer Logs
      </h1>

      <p className="text-gray-500 mb-8">
        History of all transfer operations.
      </p>

      <div className="bg-white rounded-xl shadow-md border overflow-hidden">

        <table className="w-full">

          <thead className="bg-gray-100">

            <tr>
              <th className="text-left p-4">
                Transfer Id
              </th>

              <th className="text-left p-4">
                Transfer Date
              </th>

              <th className="text-left p-4">
                Total Records
              </th>

              <th className="text-left p-4">
                Success Count
              </th>

              <th className="text-left p-4">
                Status
              </th>

            </tr>

          </thead>

          <tbody>

            {

              transferLogs.map(log => (

                <tr
                  key={log.id}
                  className="border-t hover:bg-gray-50"
                >
                  <td className="p-4" title={log.id}>
                    {log.id.substring(0, 8)}
                  </td>

                  <td className="p-4">

                    {
                      new Date(log.transferDate)
                        .toLocaleString()
                    }

                  </td>

                  <td className="p-4">
                    {log.totalRecords}
                  </td>

                  <td className="p-4">
                    {log.successCount}
                  </td>

                  <td className="p-4">

                    <StatusBadge
                      status={log.status}
                    />

                  </td>

                </tr>

              ))

            }

          </tbody>

        </table>

      </div>

    </div>

  );

}

export default TransferLogs;