import './App.css';
import {Col, Row, Container} from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'
import WaitingRoom from './components/waitingroom'
import { useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

function App() {

  const[connect, setConnection] = useState();
  const joinChatRoom = async (username, chatroom) => {
    try{
      const connect = new HubConnectionBuilder()
      .withUrl("https://localhost:7156/chat")
      .configureLogging(LogLevel.Information)
      .build();

      connect.on("JoinSpecificChatRoom", (username, msg) => {
        console.log("msg ", msg);
      })

      await connect.start();
      await connect.invoke("JoinSpecificChatRoom", {username, chatroom});
      setConnection(connect);

    }catch(e){
      console.log(e.message)
    }
  }



  return (
    <div>
      <main>
        <Container>
          <Row class='px-5 my-5'>
            <Col sm='12'>
            <h1 className='font-weight-light'>TESTTTTTTTTTTTTTTTT</h1>
            </Col>
          </Row>
          <WaitingRoom joinChatRoom={joinChatRoom}></WaitingRoom>
        </Container>
      </main>
    </div>
  );
}

export default App;
