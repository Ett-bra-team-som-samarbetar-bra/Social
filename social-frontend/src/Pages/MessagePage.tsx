import RootButton from "../Components/RootButton";

export default function MessagePage() {

    return (
        <>
        <div className="d-flex p-5 gap-5" >
            <RootButton keyLabel="L" className="small-button" onClick={() => console.log("Load older")}>Load older</RootButton>
            <RootButton keyLabel="P" className="small-button" onClick={() => console.log("Scroll up")}>Scroll up</RootButton>
            <RootButton keyLabel="N" className="small-button" onClick={() => console.log("Scroll down")}>Scroll down</RootButton>

        </div>

        </>
    );
}
