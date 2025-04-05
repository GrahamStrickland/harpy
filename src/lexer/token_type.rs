use std::slice::Iter;

#[derive(Debug, Hash, PartialEq, Eq)]
pub enum TokenType {
    LeftParen,
    RightParen,
    Comma,
    Assign,
    Plus,
    Minus,
    Asterisk,
    Slash,
    Bang,
    Question,
    Colon,
    Name,
    At,
    Eof,
}

impl TokenType {
    pub fn punctuator(&self) -> Option<char> {
        match &self {
            TokenType::LeftParen => Some('('),
            TokenType::RightParen => Some(')'),
            TokenType::Comma => Some(','),
            TokenType::Assign => Some('='),
            TokenType::Plus => Some('+'),
            TokenType::Minus => Some('-'),
            TokenType::Asterisk => Some('*'),
            TokenType::Slash => Some('/'),
            TokenType::Bang => Some('!'),
            TokenType::Question => Some('?'),
            TokenType::Colon => Some(':'),
            TokenType::At => Some('@'),
            _ => None,
        }
    }

    pub fn iterator() -> Iter<'static, TokenType> {
        static TOKEN_TYPE: [TokenType; 14] = [
            TokenType::LeftParen,
            TokenType::RightParen,
            TokenType::Comma,
            TokenType::Assign,
            TokenType::Plus,
            TokenType::Minus,
            TokenType::Asterisk,
            TokenType::Slash,
            TokenType::Bang,
            TokenType::Question,
            TokenType::Colon,
            TokenType::Name,
            TokenType::At,
            TokenType::Eof,
        ];
        TOKEN_TYPE.iter()
    }
}
