interface PostAlertMessageProps {
  message: string;
  isErrorMessage?: boolean;
}

export default function PostAlertMessage({ message, isErrorMessage = false }: PostAlertMessageProps) {
  return (
    <div className="post-box post-outline">
      <h2 className="post-title ms-1 mb-2">
        @system
      </h2>

      <div className="post-header gap-3 d-flex justify-content-between align-items-center">
        <div className="d-flex gap-3 align-items-center">
          <h4 className="post-title">[{isErrorMessage ? "ERROR" : "INFO"}]</h4>
        </div>
      </div>

      <div className="post-body mt-3 ms-1">
        <pre className="post-content">
          {message}! 根鍵
        </pre>
      </div>
    </div>
  );
}
