const JOIN_ROOM = 'JOIN_ROOM';

export function joinRoom(payload) {
  return {
    type: JOIN_ROOM,
    payload,
  };
}

export default function reducer() {

}
