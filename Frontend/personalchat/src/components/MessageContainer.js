const MessageContainer = ({messages}) => {
    return (
      <div style={{height: '400px', overflowY: 'auto'}}>
        <table className="table table-striped">
          <tbody>
            {messages.map((msg, index) => 
              <tr key={index}>
                <td>
                  {msg.username === 'SYSTEM' ? (
                    <em style={{color: 'gray'}}>{msg.msg}</em>
                  ) : (
                    <><strong>{msg.username}:</strong> {msg.msg}</>
                  )}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    );
}

export default MessageContainer;