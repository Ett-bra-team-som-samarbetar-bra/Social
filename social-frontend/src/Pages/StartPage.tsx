import RootButton from "../Components/RootButton";

function sendMessage() {
    console.log("Message sent!");
}
export default function StartPage() {
    return (<>
        <RootButton keyLabel="Enter" onClick={sendMessage} className="m-5">
            Send
        </RootButton>
    </>
    );
}