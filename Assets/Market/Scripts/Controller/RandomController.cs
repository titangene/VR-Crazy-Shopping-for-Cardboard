using System;

public class RandomController {

    // 亂數 value
    public Random random;

    /// <summary>
    /// 產生新的亂數 value
    /// </summary>
    public void GeneratorRandom() {
        // 使用 DateTime.Now.Ticks 可產生不重複的隨機亂數
        // DateTime.Now.Ticks 是指從 DateTime.MinValue 之後過了多少時間，10000000 為一秒
        random = new Random((int) DateTime.Now.Ticks);
    }

    public int UseRandom(int min, int max) {
        return random.Next(min, max + 1);
    }
}
