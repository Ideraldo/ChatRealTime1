import { useState } from "react";
import { Button, Form, InputGroup } from "react-bootstrap";

const SendMessageForm = ({sendMessage}) => {
    const[msg, setmessage] = useState('');

    return <Form onSubmit={e => {
        e.preventDefault();
        sendMessage(msg);
        setmessage('');
    }}>
    <InputGroup className="mb-3">
        <InputGroup.Text>Chat</InputGroup.Text>
        <Form.Control onChange={e => setmessage(e.target.value)} value={msg} placeholder="Type a message"></Form.Control>
        <Button variant="primary" type="submit" disabled={!msg}>Send</Button>
    </InputGroup>
    </Form>
}

export default SendMessageForm;