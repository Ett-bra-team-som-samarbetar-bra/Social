interface JsonDisplayProps {
  data: Record<string, string>;
}

export default function JsonDisplay({ data }: JsonDisplayProps) {
  return (
    <div className="json-box-container json-display-container">
      <div className=" json-display-box">
        <pre className="json-pre json-display-pre">
          {`{`}
          {Object.entries(data).map(([key, value], i, arr) => (
            <div className="json-line" key={i}>
              <span className="json-key">"{key}": </span>
              <span className="json-value">{JSON.stringify(value)}</span>
              {i < arr.length - 1 ? "," : ""}
            </div>
          ))}
          {`}`}
        </pre>
      </div>
    </div>
  );
}
