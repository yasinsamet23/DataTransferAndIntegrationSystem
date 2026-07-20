import { useEffect, useState } from "react";
import api from "../services/api";

function ErrorLogs() {

  const [errors, setErrors] = useState([]);
  const [search, setSearch] = useState("");
  const filteredErrors = errors.filter(error => {
    const searchText = search.toLowerCase();

    return (

      new Date(error.createdDate)
        .toLocaleString()
        .toLowerCase()
        .includes(searchText)

      ||

      error.transferLogId
        .toLowerCase()
        .includes(searchText)

      ||

      error.recordId
        .toLowerCase()
        .includes(searchText)

      ||

      error.errorField
        .toLowerCase()
        .includes(searchText)

      ||

      error.errorMessage
        .toLowerCase()
        .includes(searchText)

    );
  });

  useEffect(() => {

    loadErrors();

  }, []);

  const loadErrors = async () => {

    try {

      const response = await api.get("/errors");

      setErrors(response.data);

    }

    catch (error) {

      console.error(error);

    }

  };

  return (

    <div>

      <h1 className="text-3xl font-bold mb-2">
        Error Logs
      </h1>

      <p className="text-gray-500 mb-8">
        History of all transfer errors.
      </p>
      <div className="mb-6">

        <input
          type="text"
          placeholder="Search by Date, Transfer Id, Record Id, Error Field or Error Message"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="
      w-[550px]
      border
      border-gray-300
      rounded-lg
      px-4
      py-2
      bg-white
      focus:outline-none
      focus:ring-2
      focus:ring-blue-500
    "
        />

      </div>

      <div className="bg-white rounded-xl shadow-md border overflow-hidden">

        <table className="w-full">

          <thead className="bg-gray-50">

            <tr>

              <th className="text-left p-4">
                Created Date
              </th>

              <th className="text-left p-4">
                Transfer Id
              </th>

              <th className="text-left p-4">
                Record Id
              </th>

              <th className="text-left p-4">
                Error Field
              </th>

              <th className="text-left p-4">
                Error Message
              </th>

            </tr>

          </thead>

          <tbody>

            {

              filteredErrors.map((error) => (

                <tr
                  key={error.id}
                  className="border-t hover:bg-gray-50"
                >

                  <td className="p-4">

                    {
                      new Date(error.createdDate)
                        .toLocaleString()
                    }

                  </td>

                  <td
                    className="p-4"
                    title={error.transferLogId}
                  >
                    {error.transferLogId.substring(0, 8)}
                  </td>

                  <td
                    className="p-4"
                    title={error.recordId}
                  >
                    {error.recordId.substring(0, 8)}
                  </td>

                  <td className="p-4">

                    {error.errorField}

                  </td>

                  <td className="p-4">

                    {error.errorMessage}

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

export default ErrorLogs;