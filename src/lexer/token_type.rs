pub enum TokenType {
    LeftParen,
    RightParen,
    Comma,
    Assign,
    Plus,
    Minus,
    Asterisk,
    Slash,
    Caret,
    Tilde,
    Bang,
    Question,
    Colon,
    Name,
    Eof,
}

pub fn punctuator(token_type: TokenType) -> Option<char> {
    match token_type {
        TokenType::LeftParen => Some('('),
        TokenType::RightParen => Some(')'),
        TokenType::Comma => Some(','),
        TokenType::Assign => Some('='),
        TokenType::Plus => Some('+'),
        TokenType::Minus => Some('-'),
        TokenType::Asterisk => Some('*'),
        TokenType::Slash => Some('/'),
        TokenType::Caret => Some('^'),
        TokenType::Tilde => Some('~'),
        TokenType::Bang => Some('!'),
        TokenType::Question => Some('?'),
        TokenType::Colon => Some(':'),
        _ => None,
    }
}
