import React from 'react';

export function JoinRoomForm() {
  return (
    <div className="o-grid o-grid--no-gutter">
      <form className="o-grid__cell o-grid__cell--width-80 o-grid__cell--offset-10 u-centered">
        <div className="o-field o-field--icon-left u-mb-medium u-large">
          <span className="c-icon">#</span>
          <input className="c-field"
                 type="text"
                 placeholder="your-galaxy-hashtag"
          />
        </div>
        <button className="c-button c-button--brand u-large">Join</button>
      </form>
    </div>

  );
}

export default JoinRoomForm;
