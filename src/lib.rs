mod lexer;

pub use crate::lexer::lexer::Lexer;
pub use crate::lexer::token::Token;
pub use crate::lexer::token_type::TokenType;

#[cfg(test)]
mod tests {
    use super::{Lexer, Token, TokenType};

    #[test]
    fn get_type() {
        let mut token_type = TokenType::LeftParen;
        let mut result = token_type.punctuator();

        assert_eq!(result, Some('('));

        token_type = TokenType::Eof;
        result = token_type.punctuator();

        assert_eq!(result, None);
    }

    #[test]
    fn lexer() {
        let mut lexer = Lexer::new("");
        let result = lexer.next();
        let token = Token {
            token_type: TokenType::Eof,
            text: String::from(""),
        };

        assert_eq!(result, Some(token));

        let mut lexer = Lexer::new("Hello");
        let result = lexer.next();
        let token = Token {
            token_type: TokenType::Name,
            text: String::from("Hello"),
        };

        assert_eq!(result, Some(token));

        let mut lexer = Lexer::new("from + offset(time)");
        let tokens = vec![
            Token {
                token_type: TokenType::Name,
                text: String::from("from"),
            },
            Token {
                token_type: TokenType::Plus,
                text: String::from("+"),
            },
            Token {
                token_type: TokenType::Name,
                text: String::from("offset"),
            },
            Token {
                token_type: TokenType::LeftParen,
                text: String::from("("),
            },
            Token {
                token_type: TokenType::Name,
                text: String::from("time"),
            },
            Token {
                token_type: TokenType::RightParen,
                text: String::from(")"),
            },
        ];

        let mut tmp = tokens.iter().next().zip(lexer.next()).into_iter();
        while let Some((result, token)) = tmp.next() {
            assert_eq!(result, &token);
        }
    }
}
