function Toast({ result }) {

    const isSuccess = result.failedRecords === 0;
    const isFailed = result.successfulRecords === 0;

    let backgroundColor = "bg-yellow-500";
    let title = "Transfer Completed With Errors";

    if (isSuccess) {
        backgroundColor = "bg-green-600";
        title = "Transfer Completed";
    }

    if (isFailed) {
        backgroundColor = "bg-red-600";
        title = "Transfer Failed";
    }

    return (
        <div className="fixed top-5 right-5 z-50">

            <div
                className={`${backgroundColor} text-white rounded-xl shadow-2xl p-5 w-96`}
            >

                <h2 className="text-lg font-bold mb-3">
                    {title}
                </h2>

                <div className="space-y-1 text-sm">

                    <p>
                        <strong>Total Records:</strong>{" "}
                        {result.totalRecords}
                    </p>

                    <p>
                        <strong>Successful:</strong>{" "}
                        {result.successfulRecords}
                    </p>

                    <p>
                        <strong>Failed:</strong>{" "}
                        {result.failedRecords}
                    </p>

                    <hr className="my-3 opacity-30" />

                    <p>
                        {result.message}
                    </p>

                </div>

            </div>

        </div>
    );
}

export default Toast;